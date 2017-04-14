using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tomaszkiewicz.WitAi.Handlers;

namespace Tomaszkiewicz.WitAi
{
    public class WitDispatcher
    {
        private readonly WitService _service;
        private readonly Dictionary<string, IntentHandlerInfo> _intentHandlers = new Dictionary<string, IntentHandlerInfo>();
        private bool _resetRequested;
        private Dictionary<string, object> _witContext = new Dictionary<string, object>();
        private string _sessionId;

        public WitDispatcher(string authToken)
        {
            _service = new WitService(authToken);

            ResetConversation();
        }

        public void RegisterIntentHandler(string intent, IIntentHandler handler)
        {
            var handlerInfo = new IntentHandlerInfo(handler);

            _intentHandlers[intent] = handlerInfo;
        }

        public async Task Dispatch(string query)
        {
            while (true)
            {
                var serializedContext = JsonConvert.SerializeObject(_witContext);

                Debug.WriteLine("******************************************************************************");
                Debug.WriteLine($"Session Id: {_sessionId}");
                Debug.WriteLine($"Message: {query}");
                Debug.WriteLine("Context:");
                Debug.WriteLine(serializedContext);

                var result = await _service.QueryAsync(query, _sessionId, serializedContext);

                Debug.WriteLine("Result:");
                Debug.WriteLine(JsonConvert.SerializeObject(result));

                var intent = result.Entities.First(x => x.Key == "intent").Value.First().Value;
                
                var intentHandlerInfo = _intentHandlers[intent];

                switch (result.Type)
                {
                    case "action":
                        Debug.WriteLine($"ACTION {result.Action}");
                        await DispatchToActionHandler(intentHandlerInfo, result);
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

                        if (_resetRequested)
                            await ResetConversation();

                        _resetRequested = false;

                        return;

                    default:
                        throw new InvalidOperationException("Unsupported Wit result type.");
                }
            }
        }

        private Task ResetConversation()
        {
            Debug.WriteLine("RESET");

            _sessionId = Guid.NewGuid().ToString();
            _witContext = new Dictionary<string, object>();

            return Task.CompletedTask;
        }

        private async Task DispatchToActionHandler(IntentHandlerInfo intentHandlerInfo, WitResult result)
        {
            var actionHandlerInfo = intentHandlerInfo.GetActionHandler(result.Action);

            if (actionHandlerInfo != null)
            {
                CheckIfContextResetNecessary(actionHandlerInfo);

                MergeEntitiesIntoContext(actionHandlerInfo, result);

                LoadDataFromPrivateConversationDataIntoContext(actionHandlerInfo);

                var call = CheckRequiredEntities(actionHandlerInfo);

                // ...and finally, when all entities are present in context - make a call
                if (call)
                {
                    bool requestReset = false;

                    await actionHandlerInfo.Handler(_witContext, result, ref requestReset);

                    if (requestReset)
                        _resetRequested = true;
                }
            }
            else
                throw new Exception("No default action handler found.");
        }

        private void LoadDataFromPrivateConversationDataIntoContext(ActionHandlerInfo handlerInfo)
        {
            // TODO

            //foreach (var entity in handlerInfo.WitLoadPrivateConversationData)
            //    if (!WitContext.ContainsKey(entity.Name) && context.PrivateConversationData.ContainsKey(entity.Name))
            //        WitContext[entity.Name] = context.PrivateConversationData.Get<object>(entity.Name);
        }

        /// <summary>
        /// Merge all or selected wit result entities to wit context
        /// </summary>
        /// <param name="handlerInfo"></param>
        /// <param name="result"></param>
        private void MergeEntitiesIntoContext(ActionHandlerInfo handlerInfo, WitResult result)
        {
            if ((!handlerInfo.MergeAll && !handlerInfo.WitMerges.Any()) || result.Entities == null)
                return;

            var entitiesToMerge = result.Entities.Where(e => e.Key != "intent" && (handlerInfo.MergeAll || handlerInfo.WitMerges.Select(s => s.Name).Contains(e.Key)));

            foreach (var entity in entitiesToMerge)
                _witContext[entity.Key] = entity.Value.FirstOrDefault()?.Value;
        }

        private void CheckIfContextResetNecessary(ActionHandlerInfo handlerInfo)
        {
            if (handlerInfo.WitReset)
                _resetRequested = true;
        }

        /// <summary>
        /// Check if all required entities are in context already. Otherwise update context with information about missing entities.
        /// </summary>
        /// <param name="handlerInfo"></param>
        /// <returns></returns>
        private bool CheckRequiredEntities(ActionHandlerInfo handlerInfo)
        {
            var call = true;

            foreach (var entity in handlerInfo.WitRequireEntities)
            {
                if (!_witContext.ContainsKey(entity.Name))
                {
                    _witContext[$"{entity.Name}Missing"] = true;

                    call = false;

                    break;
                }

                _witContext.Remove($"{entity.Name}Missing");
            }
            return call;
        }

    }
}
