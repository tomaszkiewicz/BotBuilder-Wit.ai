using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Tomaszkiewicz.WitAi.Attributes;
using Tomaszkiewicz.WitAi.Interfaces;

namespace Tomaszkiewicz.WitAi.SampleBot.Handlers
{
    internal class GreetingsHandler : DefaultBotHandler, IIntentHandler
    {
        public GreetingsHandler(Activity activity) : base(activity)
        {
        }

        [WitAction("greetings")]
        public Task<bool> Greetings(Dictionary<string, object> witContext, WitResult witResult, IWitPrivateConversationDataPersistence persistence)
        {
            witContext["user"] = "Test user";

            return Task.FromResult(false);
        }

        public string Intent => "greetings";
    }
}