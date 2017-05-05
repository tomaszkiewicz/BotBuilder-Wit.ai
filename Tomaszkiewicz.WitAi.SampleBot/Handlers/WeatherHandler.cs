using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Tomaszkiewicz.WitAi.Attributes;
using Tomaszkiewicz.WitAi.Interfaces;

namespace Tomaszkiewicz.WitAi.SampleBot.Handlers
{
    internal class WeatherHandler : DefaultBotHandler, IIntentHandler
    {
        public WeatherHandler(Activity activity) : base(activity)
        {
        }

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