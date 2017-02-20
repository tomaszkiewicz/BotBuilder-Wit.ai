using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Wit.Bot.Framework.Builder.Attributes;
using Wit.Bot.Framework.Builder.Dialogs;
using Wit.Bot.Framework.Builder.Models;

namespace Wit.Bot.Framework.Builder.Sample.Dialogs
{
    [Serializable]
    [WitModel("VZQQJVTB2E5DBMXL2RXSS73CX75H4ABO")]
    public class MainDialog : WitDialog<object>
    {
        [WitAction("greetings")]
        public Task Greetings(IDialogContext context, IAwaitable<IMessageActivity> message, WitResult witResult)
        {
            WitContext["user"] = "John";

            return Task.CompletedTask;
        }

        [WitAction("getForecast")]
        public async Task GetForecast(IDialogContext context, IAwaitable<IMessageActivity> message, WitResult witResult)
        {
            if (!witResult.Entities.ContainsKey("location"))
            {
                WitContext["missingLocation"] = true;

                return;
            }

            if (!witResult.Entities.ContainsKey("datetime"))
            {
                WitContext["missingDate"] = true;

                return;
            }

            WitContext["forecast"] = "Sunny";
        }
    }
}