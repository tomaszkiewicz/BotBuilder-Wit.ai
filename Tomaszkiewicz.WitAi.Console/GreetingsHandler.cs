using System.Collections.Generic;
using System.Threading.Tasks;
using Tomaszkiewicz.WitAi.Attributes;

namespace Tomaszkiewicz.WitAi.Console
{
    internal class GreetingsHandler : ConsoleHandler
    {
        [WitAction("greetings")]
        public Task Greetings(Dictionary<string, object> witContext, WitResult witResult, ref bool requestReset)
        {
            witContext["user"] = "Test user";

            return Task.CompletedTask;
        }
    }
}