using System.Collections.Generic;
using System.Threading.Tasks;
using Tomaszkiewicz.WitAi.Attributes;

namespace Tomaszkiewicz.WitAi.Console
{
    internal class WeatherHandler : ConsoleHandler
    {
        [WitAction("getForecast")]
        [WitRequireEntity("location")]
        [WitLoadPrivateConversationData("location")]
        [WitRequireEntity("datetime")]
        [WitMergeAll]
        public Task Greetings(Dictionary<string, object> witContext, WitResult witResult, ref bool requestReset)
        {
            witContext["forecast"] = "sunny";

            return Task.CompletedTask;
        }
    }
}