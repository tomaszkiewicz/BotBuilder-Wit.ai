using System.Collections.Generic;
using System.Threading.Tasks;
using Tomaszkiewicz.WitAi.Attributes;
using Tomaszkiewicz.WitAi.Interfaces;

namespace Tomaszkiewicz.WitAi.Console
{
    internal class GreetingsHandler : ConsoleHandler, IIntentHandler
    {
        [WitAction("greetings")]
        public Task<bool> Greetings(Dictionary<string, object> witContext, WitResult witResult, IWitPrivateConversationDataPersistence persistence)
        {
            witContext["user"] = "Test user";

            return Task.FromResult(false);
        }

        public string Intent => "greetings";
    }
}