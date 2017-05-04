using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tomaszkiewicz.WitAi.Attributes;
using Tomaszkiewicz.WitAi.Exceptions;
using Tomaszkiewicz.WitAi.Handlers;
using Tomaszkiewicz.WitAi.Interfaces;

namespace Tomaszkiewicz.WitAi
{
    internal class IntentHandlerInfo
    {
        public IDefaultIntentHandler Handler { get; }

        readonly Dictionary<string, ActionHandlerInfo> _actionHandlers;

        public IntentHandlerInfo(IDefaultIntentHandler handler)
        {
            Handler = handler;

            _actionHandlers = EnumerateHandlersInfo(handler).ToDictionary(x => x.Key, y => y.Value);
        }

        public ActionHandlerInfo GetActionHandler(string action)
        {
            if (!_actionHandlers.ContainsKey(action))
                throw new NoActionHandlerException($"No handler for action \"{action}\" in intent handler {Handler.GetType().FullName}.");

            return _actionHandlers[action];
        }

        private static IEnumerable<KeyValuePair<string, ActionHandlerInfo>> EnumerateHandlersInfo(object obj)
        {
            var methods = obj.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var method in methods)
            {
                var actions = method.GetCustomAttributes<WitActionAttribute>(true).ToArray();
                var entities = method.GetCustomAttributes<WitRequireEntityAttribute>(true).ToArray();
                var reset = method.GetCustomAttributes<WitResetAttribute>(true).Any();
                var merges = method.GetCustomAttributes<WitMergeAttribute>(true).ToList();
                var mergeAll = method.GetCustomAttributes<WitMergeAllAttribute>(true).Any();
                var loadPrivateConversationData = method.GetCustomAttributes<WitLoadPrivateConversationData>(true);

                var handlerInfo = new ActionHandlerInfo()
                {
                    MergeAll = mergeAll,
                    WitMerges = merges.ToArray(),
                    WitReset = reset,
                    WitRequireEntities = entities.ToArray(),
                    WitLoadPrivateConversationData = loadPrivateConversationData.ToArray()
                };

                ActionHandler actionHandler = null;

                try
                {
                    actionHandler = (ActionHandler)Delegate.CreateDelegate(typeof(ActionHandler), obj, method, false);
                }
                catch (ArgumentException)
                {
                    // "Cannot bind to the target method because its signature or security transparency is not compatible with that of the delegate type."
                    // https://github.com/Microsoft/BotBuilder/issues/634
                    // https://github.com/Microsoft/BotBuilder/issues/435
                }

                // fall back for compatibility
                /*
                if (actionHandler == null)
                {
                    try
                    {
                        var handler = (ActionHandler)Delegate.CreateDelegate(typeof(ActionHandler), obj, method, false);

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
                */

                if (actionHandler != null)
                {
                    handlerInfo.Handler = actionHandler;

                    var actionNames = actions.Select(i => i.ActionName).DefaultIfEmpty(method.Name);

                    foreach (var actionName in actionNames)
                        yield return new KeyValuePair<string, ActionHandlerInfo>(actionName, handlerInfo);
                }
                else
                {
                    if (actions.Any())
                        throw new InvalidOperationException(string.Join(";", actions.Select(i => i.ActionName)));
                }
            }
        }
    }
}