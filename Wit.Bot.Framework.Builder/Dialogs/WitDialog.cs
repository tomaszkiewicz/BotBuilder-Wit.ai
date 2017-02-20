using System;
using System.Reflection;
using System.Collections.Generic;
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
        private const string WitSessionIdKey = "witSessionId";
        private const string WitContextKey = "witContext";

        public IDictionary<string, object> WitContext;
        public string WitSessionId;

        protected readonly IWitService Service;

        [NonSerialized]
        protected Dictionary<string, ActionActivityHandler> HandlerByAction;

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

            WitContext = context.PrivateConversationData.Get<IDictionary<string, object>>(WitContextKey);
            WitSessionId = context.PrivateConversationData.Get<string>(WitSessionIdKey);

            await MessageHandler(context, item);
        }

        private async Task MessageHandler(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            while (true)
            {
                context.PrivateConversationData.SetValue(WitContextKey, WitContext);
                context.PrivateConversationData.SetValue(WitSessionIdKey, WitSessionId);

                var message = await item;
                var messageText = await GetWitQueryTextAsync(context, message);

                var jsonContext = new JavaScriptSerializer().Serialize(WitContext);

                var result = await Service.QueryAsync(messageText, WitSessionId, jsonContext, context.CancellationToken);

                switch (result.Type)
                {
                    case "action":
                        await DispatchToActionHandler(context, item, result);
                        continue;

                    case "msg":
                        if (result.QuickReplies != null && result.QuickReplies.Any())
                            await context.PostQuickRepliesAsync(result.QuickReplies, message: result.Message);
                        else
                            await context.PostAsync(result.Message);

                        continue;

                    case "stop":
                        await ResetConversation(context);
                        break;

                    default:
                        throw new UnsupportedWitActionException($"Action {result.Type} is not supported");
                }

                break;
            }
        }

        private static Task ResetConversation(IDialogContext context)
        {
            context.PrivateConversationData.SetValue<IDictionary<string, object>>(WitContextKey, new Dictionary<string, object>());
            context.PrivateConversationData.SetValue(WitSessionIdKey, Guid.NewGuid().ToString());

            return Task.CompletedTask;
        }

        protected virtual async Task DispatchToActionHandler(IDialogContext context, IAwaitable<IMessageActivity> item, WitResult result)
        {
            if (HandlerByAction == null)
                HandlerByAction = new Dictionary<string, ActionActivityHandler>(GetHandlersByAction());

            ActionActivityHandler handler;

            if (string.IsNullOrEmpty(result.Action) || !HandlerByAction.TryGetValue(result.Action, out handler))
                handler = HandlerByAction[string.Empty];

            if (handler != null)
                await handler(context, item, result);
            else
                throw new System.Exception("No default action handler found.");
        }

        protected virtual IDictionary<string, ActionActivityHandler> GetHandlersByAction()
        {
            return WitDialog.EnumerateHandlers(this).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        protected virtual Task<string> GetWitQueryTextAsync(IDialogContext context, IMessageActivity message)
        {
            return Task.FromResult(message.Text);
        }
    }

    internal static class WitDialog
    {
        public static IEnumerable<KeyValuePair<string, ActionActivityHandler>> EnumerateHandlers(object dialog)
        {
            var methods = dialog.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var method in methods)
            {
                var actions = method.GetCustomAttributes<WitActionAttribute>(true).ToArray();

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
                    var actionNames = actions.Select(i => i.ActionName).DefaultIfEmpty(method.Name);

                    foreach (var actionName in actionNames)
                        yield return new KeyValuePair<string, ActionActivityHandler>(actionName, actionHandler);
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