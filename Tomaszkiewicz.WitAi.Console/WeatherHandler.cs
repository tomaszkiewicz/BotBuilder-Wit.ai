using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tomaszkiewicz.WitAi.Attributes;
using Tomaszkiewicz.WitAi.Interfaces;

namespace Tomaszkiewicz.WitAi.Console
{
    internal class WeatherHandler : ConsoleHandler, IIntentHandler
    {
        [WitAction("getForecast")]
        [WitRequireEntity("location")]
        [WitLoadPrivateConversationData("location")]
        [WitRequireEntity("datetime")]
        [WitMergeAll]
        public async Task<bool> GetForecast(Dictionary<string, object> witContext, WitResult witResult, IWitPrivateConversationDataPersistence persistence)
        {
            await persistence.SetPrivateConversationData("location", witContext["location"]);

            witContext["forecast"] = "sunny";

            return false;
        }

        public string Intent => "weather";
    }
}