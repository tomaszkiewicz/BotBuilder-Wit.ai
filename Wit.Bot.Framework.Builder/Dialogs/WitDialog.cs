using System;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Wit.Bot.Framework.Builder.Models;
using System.Linq;
using System.Web.Script.Serialization;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using Wit.Bot.Framework.Builder.Attributes;
using Wit.Bot.Framework.Builder.Exception;
using Wit.Bot.Framework.Builder.Extensions;
using Wit.Bot.Framework.Builder.Handlers;
using Wit.Bot.Framework.Builder.Interfaces;

namespace Wit.Bot.Framework.Builder.Dialogs
{
    [Serializable]
    public class WitDialog<TResult> : IDialog<TResult>
    {
        protected const string WitSessionIdKey = "witSessionId";
        protected const string WitContextKey = "witContext";

        public Dictionary<string, object> WitContext;
        public string WitSessionId;

        protected readonly IWitService Service;

        [NonSerialized]
        protected Dictionary<string, ActionActivityHandlerInfo> HandlerInfoByAction;

        private bool _resetRequested;

        public IWitService MakeServiceFromAttributes()
        {
            var witModels = GetType().GetCustomAttributes<WitModelAttribute>(true).ToArray();

            if (witModels.Length > 1)
                throw new WitModelDisambiguationException("WitDialog does not support more than one WitModel per instance");

            return new WitService(witModels[0]);
        }

        public WitDialog()
        {
            Service = MakeServiceFromAttributes();

            SetField.NotNull(out Service, nameof(Service), Service);
        }

        public virtual async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceived);
        }

        protected virtual async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            if (!context.PrivateConversationData.ContainsKey(WitSessionIdKey))
                await ResetConversation(context);

            WitContext = context.PrivateConversationData.Get<Dictionary<string, object>>(WitContextKey);
            WitSessionId = context.PrivateConversationData.Get<string>(WitSessionIdKey);

            await MessageHandler(context, item);
        }

        private async Task MessageHandler(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = await item;
            var messageText = await GetWitQueryTextAsync(context, message);

            while (true)
            {
                context.PrivateConversationData.SetValue(WitContextKey, WitContext);
                context.PrivateConversationData.SetValue(WitSessionIdKey, WitSessionId);

                var jsonContext = new JavaScriptSerializer().Serialize(WitContext);

                Debug.WriteLine("******************************************************************************");
                Debug.WriteLine($"Session Id: {WitSessionId}");
                Debug.WriteLine($"Message: {messageText}");
                Debug.WriteLine("Context:");
                Debug.WriteLine(jsonContext);

                var result = await Service.QueryAsync(messageText, WitSessionId, jsonContext, context.CancellationToken);

                messageText = null; // reset query for subsequent calls as it is not recommended by wit.ai to send it again

                Debug.WriteLine("Result:");
                Debug.WriteLine(new JavaScriptSerializer().Serialize(result));

                switch (result.Type)
                {
                    case "action":
                        Debug.WriteLine($"ACTION {result.Action}");
                        await DispatchToActionHandler(context, item, result);
                        continue;

                    case "msg":
                        Debug.WriteLine($"SAY: {result.Message}");

                        if (result.QuickReplies != null && result.QuickReplies.Any())
                            await context.PostQuickRepliesAsync(result.QuickReplies, message: result.Message);
                        else
                            await context.PostAsync(result.Message);

                        continue;

                    case "stop":
                        Debug.WriteLine("STOP");

                        if (_resetRequested)
                            await ResetConversation(context);

                        _resetRequested = false;

                        break;

                    default:
                        throw new UnsupportedWitActionException($"Action {result.Type} is not supported");
                }

                break;
            }
        }

        private static Task ResetConversation(IDialogContext context)
        {
            Debug.WriteLine("RESET");

            context.PrivateConversationData.SetValue<IDictionary<string, object>>(WitContextKey, new Dictionary<string, object>());
            context.PrivateConversationData.SetValue(WitSessionIdKey, Guid.NewGuid().ToString());

            return Task.CompletedTask;
        }

        protected virtual async Task DispatchToActionHandler(IDialogContext context, IAwaitable<IMessageActivity> item, WitResult result)
        {
            if (HandlerInfoByAction == null)
                HandlerInfoByAction = new Dictionary<string, ActionActivityHandlerInfo>(GetHandlersInfoByAction());

            ActionActivityHandlerInfo handlerInfo;

            if (string.IsNullOrEmpty(result.Action) || !HandlerInfoByAction.TryGetValue(result.Action, out handlerInfo))
                handlerInfo = HandlerInfoByAction[string.Empty];

            if (handlerInfo != null)
            {
                // check if action is tagged with WitReset attribute
                if (handlerInfo.WitReset)
                    _resetRequested = true;

                // merge all or selected results to wit context
                if ((handlerInfo.MergeAll || handlerInfo.WitMerges.Any()) && result.Entities != null)
                {
                    var entitiesToMerge = result.Entities.Where(e => e.Key != "intent" && (handlerInfo.MergeAll || handlerInfo.WitMerges.Select(s => s.Name).Contains(e.Key)));

                    foreach (var entity in entitiesToMerge)
                        WitContext[entity.Key] = entity.Value.FirstOrDefault()?.Value;
                }

                // check if all required entities are in context
                var call = true;

                foreach (var entity in handlerInfo.WitRequireEntities)
                {
                    if (!WitContext.ContainsKey(entity.Name))
                    {
                        WitContext[$"{entity.Name}Missing"] = true;

                        call = false;

                        break;
                    }

                    WitContext.Remove($"{entity.Name}Missing");
                }

                // ...and finally, when all entities are present in context - make a call
                if (call)
                    await handlerInfo.Handler(context, item, result);
            }
            else
                throw new System.Exception("No default action handler found.");
        }

        protected virtual IDictionary<string, ActionActivityHandlerInfo> GetHandlersInfoByAction()
        {
            return WitDialog.EnumerateHandlersInfo(this).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        protected virtual Task<string> GetWitQueryTextAsync(IDialogContext context, IMessageActivity message)
        {
            return Task.FromResult(message.Text);
        }
    }

    internal static class WitDialog
    {
        public static IEnumerable<KeyValuePair<string, ActionActivityHandlerInfo>> EnumerateHandlersInfo(object dialog)
        {
            var methods = dialog.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var method in methods)
            {
                var actions = method.GetCustomAttributes<WitActionAttribute>(true).ToArray();
                var entities = method.GetCustomAttributes<WitRequireEntityAttribute>(true).ToArray();
                var reset = method.GetCustomAttributes<WitResetAttribute>(true).Any();
                var merges = method.GetCustomAttributes<WitMergeAttribute>(true).ToList();
                var mergeAll = method.GetCustomAttributes<WitMergeAllAttribute>(true).Any();
                
                var handlerInfo = new ActionActivityHandlerInfo()
                {
                    MergeAll = mergeAll,
                    WitMerges = merges.ToArray(),
                    WitReset = reset,
                    WitRequireEntities = entities.ToArray()
                };

                ActionActivityHandler actionHandler = null;

                try
                {
                    actionHandler = (ActionActivityHandler)Delegate.CreateDelegate(typeof(ActionActivityHandler), dialog, method, false);
                }
                catch (ArgumentException)
                {
                    // "Cannot bind to the target method because its signature or security transparency is not compatible with that of the delegate type."
                    // https://github.com/Microsoft/BotBuilder/issues/634
                    // https://github.com/Microsoft/BotBuilder/issues/435
                }

                // fall back for compatibility
                if (actionHandler == null)
                {
                    try
                    {
                        var handler = (ActionHandler)Delegate.CreateDelegate(typeof(ActionHandler), dialog, method, false);

                        if (handler != null)
                        {
                            // thunk from new to old delegate type
                            actionHandler = (context, message, result) => handler(context, result);
                        }
                    }
                    catch (ArgumentException)
                    {
                        // "Cannot bind to the target method because its signature or security transparency is not compatible with that of the delegate type."
                        // https://github.com/Microsoft/BotBuilder/issues/634
                        // https://github.com/Microsoft/BotBuilder/issues/435
                    }
                }

                if (actionHandler != null)
                {
                    handlerInfo.Handler = actionHandler;

                    var actionNames = actions.Select(i => i.ActionName).DefaultIfEmpty(method.Name);

                    foreach (var actionName in actionNames)
                        yield return new KeyValuePair<string, ActionActivityHandlerInfo>(actionName, handlerInfo);
                }
                else
                {
                    if (actions.Any())
                        throw new InvalidIntentHandlerException(string.Join(";", actions.Select(i => i.ActionName)), method);
                }
            }
        }
    }
}