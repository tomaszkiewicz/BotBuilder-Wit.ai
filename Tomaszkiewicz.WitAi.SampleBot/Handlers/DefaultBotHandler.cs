using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Connector;
using Tomaszkiewicz.BotFramework.Extensions;
using Tomaszkiewicz.WitAi.Attributes;
using Tomaszkiewicz.WitAi.Interfaces;

namespace Tomaszkiewicz.WitAi.SampleBot.Handlers
{
    public class DefaultBotHandler : IDefaultIntentHandler
    {
        private readonly Activity _activity;
        private readonly ConnectorClient _connector;

        public DefaultBotHandler(Activity activity)
        {
            _activity = activity;
            _connector = activity.CreateConnectorClient();
        }

        public async Task Say(string text)
        {
            await _connector.Conversations.SendToConversationAsync(_activity.CreateReply(text));
        }

        public async Task QuickReplies(string text, string[] quickReplies)
        {
            await _connector.Conversations.SendToConversationAsync((Activity)_activity.MakeQuickReplies(quickReplies, text));
        }

        [WitAction("reset")]
        [WitReset]
        public Task<bool> Reset(Dictionary<string, object> witContext, WitResult witResult, IWitPrivateConversationDataPersistence persistence)
        {
            return Task.FromResult(false);
        }
    }
}