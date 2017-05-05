using Microsoft.Bot.Connector;
using Tomaszkiewicz.WitAi.Interfaces;

namespace Tomaszkiewicz.WitAi.SampleBot.Handlers
{
    internal class ThanksHandler : DefaultBotHandler, IIntentHandler
    {
        public string Intent => "thanks";

        public ThanksHandler(Activity activity) : base(activity)
        {
        }
    }
}