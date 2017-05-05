using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tomaszkiewicz.WitAi.Exceptions;
using Tomaszkiewicz.WitAi.Handlers;
using Tomaszkiewicz.WitAi.Interfaces;

namespace Tomaszkiewicz.WitAi
{
    public class WitDispatcher
    {
        private readonly IWitPersistence _persistence;
        private readonly WitService _service;
        private readonly Dictionary<string, IntentHandlerInfo> _intentHandlers = new Dictionary<string, IntentHandlerInfo>();

        public WitDispatcher(string authToken, IWitPersistence persistence)
        {
            _persistence = persistence;
            _service = new WitService(authToken);
        }

        public void SetDefaultHandler(IDefaultIntentHandler handler)
        {
            _intentHandlers[""] = new IntentHandlerInfo(handler);
        }

        public void RegisterIntentHandler(IIntentHandler handler)
        {
            if (_intentHandlers.ContainsKey(handler.Intent))
                throw new WitDispatcherException($"There is already a handler registered for intent {handler.Intent}");

            var handlerInfo = new IntentHandlerInfo(handler);

            _intentHandlers[handler.Intent] = handlerInfo;
        }

        public async Task Dispatch(string query)
        {
            while (true)
            {
                var witContext = await _persistence.GetWitContext();
                var sessionId = await _persistence.GetSessionId();
                var intent = await _persistence.GetIntent();
                var resetRequested = await _persistence.GetResetRequested();

                if (witContext == null && sessionId == null)
                {
                    await ResetConversation();
                    continue;
                }

                var serializedContext = JsonConvert.SerializeObject(witContext);

                DebugDisplayOutgoing(query, witContext, sessionId);

                var result = await _service.QueryAsync(query, sessionId, serializedContext);
                
                if (result.Entities != null && result.Entities.Any(x => x.Key == "intent"))
                {
                    var currentIntent = result.Entities.First(x => x.Key == "intent").Value.First().Value;

                    if (!string.IsNullOrWhiteSpace(currentIntent))
                    {
                        await _persistence.SetIntent(currentIntent);

                        if (currentIntent != intent && !string.IsNullOrWhiteSpace(intent))
                        {
                            Debug.WriteLine($"Intent changed from {intent} to {currentIntent}, resetting conversation and querying again.");
                            await ResetConversation();

                            continue;
                        }

                        intent = currentIntent;
                    }
                }
                
                query = null;

                DebugDisplayIncoming(intent, result);

                if (intent == null && !_intentHandlers.ContainsKey(""))
                    throw new NoIntentDetectedException();

                if (intent == null)
                {
                    intent = "";
                    await _persistence.SetIntent("");
                }

                if (!_intentHandlers.ContainsKey(intent))
                {
                    if (result.Type == "action")
                    {
                        await _persistence.SetResetRequested(true);

                        throw new WitDispatcherException($"No handler registered for intent \"{intent}\"");
                    }

                    intent = "";
                    await _persistence.SetIntent("");
                    
                }

                var intentHandlerInfo = _intentHandlers[intent];

                switch (result.Type)
                {
                    case "action":
                        Debug.WriteLine($"ACTION {result.Action}");
                        await DispatchToActionHandler(intentHandlerInfo, result, witContext);
                        continue;

                    case "msg":
                        Debug.WriteLine($"SAY: {result.Message}");

                        if (result.QuickReplies != null && result.QuickReplies.Any())
                            await intentHandlerInfo.Handler.QuickReplies(result.Message, result.QuickReplies);
                        else
                            await intentHandlerInfo.Handler.Say(result.Message);

                        continue;

                    case "stop":
                        Debug.WriteLine("STOP");

                        if (resetRequested)
                        {
                            await _persistence.SetIntent(null);
                            await ResetConversation();
                        }

                        await _persistence.SetResetRequested(false);

                        return;

                    default:
                        throw new InvalidOperationException("Unsupported Wit result type.");
                }
            }
        }

        private static void DebugDisplayIncoming(string intent, WitResult result)
        {
            Debug.WriteLine("*************************** INCOMING *******************************");
            Debug.WriteLine($"Intent: {intent}");

            if (!string.IsNullOrWhiteSpace(result.Action))
                Debug.WriteLine($"Action: {result.Action}");

            if (!string.IsNullOrWhiteSpace(result.Message))
                Debug.WriteLine($"Message: {result.Message}");

            if (result.QuickReplies != null && result.QuickReplies.Any())
                Debug.WriteLine($"Quick replies: {string.Join(", ", result.QuickReplies)}");

            if (result.Entities != null && result.Entities.Any(x => x.Key != "intent"))
            {
                Debug.WriteLine("Entities:");

                foreach (var entity in result.Entities.Where(x => x.Key != "intent"))
                    Debug.WriteLine($"\t{entity.Key}: {entity.Value.First().Value}");
            }

            //Debug.WriteLine(JsonConvert.SerializeObject(result));
            //Debug.WriteLine("********************************************************************");
        }

        private void DebugDisplayOutgoing(string query, WitContext witContext, string sessionId)
        {
            Debug.WriteLine("*************************** OUTGOING *******************************");
            Debug.WriteLine($"Session Id: {sessionId}");

            if (!string.IsNullOrWhiteSpace(query))
                Debug.WriteLine($"Message: {query}");

            if (witContext.Any())
            {
                Debug.WriteLine("Context: ");

                foreach (var kvp in witContext)
                    Debug.WriteLine($"\t{kvp.Key}: {kvp.Value}");
            }
            //Debug.WriteLine("********************************************************************");
        }

        private async Task ResetConversation()
        {
            Debug.WriteLine("RESET");

            await _persistence.SetSessionId(Guid.NewGuid().ToString());
            await _persistence.SetWitContext(new WitContext());
        }

        private async Task DispatchToActionHandler(IntentHandlerInfo intentHandlerInfo, WitResult result, WitContext witContext)
        {
            ActionHandlerInfo actionHandlerInfo;

            try
            {
                actionHandlerInfo = intentHandlerInfo.GetActionHandler(result.Action);
            }
            catch (NoActionHandlerException)
            {
                await _persistence.SetResetRequested(true);

                throw;
            }

            await CheckIfContextResetNecessary(actionHandlerInfo);

            MergeEntitiesIntoContext(actionHandlerInfo, result, witContext);

            await LoadDataFromPrivateConversationDataIntoContext(actionHandlerInfo, witContext);

            var call = CheckRequiredEntities(actionHandlerInfo, witContext);

            // ...and finally, when all entities are present in context - make a call
            if (call)
            {
                var requestReset = await actionHandlerInfo.Handler(witContext, result, _persistence);

                if (requestReset)
                    await _persistence.SetResetRequested(true);
            }

            await _persistence.SetWitContext(witContext);
        }

        private async Task LoadDataFromPrivateConversationDataIntoContext(ActionHandlerInfo handlerInfo, WitContext witContext)
        {
            foreach (var entity in handlerInfo.WitLoadPrivateConversationData)
            {
                object data;
                if (!witContext.ContainsKey(entity.Name) && await _persistence.TryGetPrivateConversationData(entity.Name, out data))
                    witContext[entity.Name] = data;
            }
        }

        /// <summary>
        /// Merge all or selected wit result entities to wit context
        /// </summary>
        /// <param name="handlerInfo"></param>
        /// <param name="result"></param>
        /// <param name="witContext"></param>
        private void MergeEntitiesIntoContext(ActionHandlerInfo handlerInfo, WitResult result, WitContext witContext)
        {
            if ((!handlerInfo.MergeAll && !handlerInfo.WitMerges.Any()) || result.Entities == null)
                return;

            var entitiesToMerge = result.Entities.Where(e => e.Key != "intent" && (handlerInfo.MergeAll || handlerInfo.WitMerges.Select(s => s.Name).Contains(e.Key)));

            foreach (var entity in entitiesToMerge)
                witContext[entity.Key] = entity.Value.FirstOrDefault()?.Value;
        }

        private async Task CheckIfContextResetNecessary(ActionHandlerInfo handlerInfo)
        {
            if (handlerInfo.WitReset)
                await _persistence.SetResetRequested(true);

        }

        /// <summary>
        /// Check if all required entities are in context already. Otherwise update context with information about missing entities.
        /// </summary>
        /// <param name="handlerInfo"></param>
        /// <param name="witContext"></param>
        /// <returns></returns>
        private bool CheckRequiredEntities(ActionHandlerInfo handlerInfo, WitContext witContext)
        {
            var call = true;

            foreach (var entity in handlerInfo.WitRequireEntities)
            {
                if (!witContext.ContainsKey(entity.Name))
                {
                    witContext[$"{entity.Name}Missing"] = true;

                    call = false;

                    break;
                }

                witContext.Remove($"{entity.Name}Missing");
            }
            return call;
        }

    }
}