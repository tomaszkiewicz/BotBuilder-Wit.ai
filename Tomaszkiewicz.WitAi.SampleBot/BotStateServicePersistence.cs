using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace Tomaszkiewicz.WitAi.SampleBot
{
    public class BotStateServicePersistence : IWitPersistence
    {
        private readonly BotData _botData;

        public static Task<BotData> GetBotData(Activity activity)
        {
            return activity.GetStateClient().BotState.GetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id);
        }

        public static Task SaveBotData(Activity activity, BotData botData)
        {
            return activity.GetStateClient().BotState.SetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id, botData);
        }

        public BotStateServicePersistence(BotData botData)
        {
            _botData = botData;
        }

        public Task<bool> TryGetPrivateConversationData(string name, out object data)
        {
            var obj = _botData.GetProperty<object>("private-" + name);

            data = obj;

            return Task.FromResult(obj != null);
        }

        public Task SetPrivateConversationData(string name, object data)
        {
            _botData.SetProperty("private-" + name, data);

            return Task.CompletedTask;
        }

        public Task<WitContext> GetWitContext()
        {
            return Task.FromResult(_botData.GetProperty<WitContext>("witContext"));
        }

        public Task<string> GetSessionId()
        {
            return Task.FromResult(_botData.GetProperty<string>("sessionId"));
        }

        public Task SetIntent(string intent)
        {
            _botData.SetProperty("intent", intent);

            return Task.CompletedTask;
        }

        public Task<string> GetIntent()
        {
            return Task.FromResult(_botData.GetProperty<string>("intent"));
        }

        public Task<bool> GetResetRequested()
        {
            return Task.FromResult(_botData.GetProperty<bool>("resetRequested"));
        }

        public Task SetResetRequested(bool resetRequested)
        {
            _botData.SetProperty("resetRequested", resetRequested);

            return Task.CompletedTask;
        }

        public Task SetSessionId(string sessionId)
        {
            _botData.SetProperty("sessionId", sessionId);

            return Task.CompletedTask;
        }

        public Task SetWitContext(WitContext witContext)
        {
            _botData.SetProperty("witContext", witContext);

            return Task.CompletedTask;
        }
    }
}