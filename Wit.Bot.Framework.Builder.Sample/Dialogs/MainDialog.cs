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
        public async Task Greetings(IDialogContext context, IAwaitable<IMessageActivity> message, WitResult witResult)
        {
            WitContext["user"] = (await message).From.Name;
        }

        [WitAction("getForecast")]
        [WitRequireEntity("location")]
        [WitRequireEntity("datetime")]
        [WitMergeAll]
        public async Task GetForecast(IDialogContext context, IAwaitable<IMessageActivity> message, WitResult witResult)
        {
            WitContext["forecast"] = "sunny";
        }

        [WitAction("reset")]
        [WitReset]
        public Task Reset(IDialogContext context, IAwaitable<IMessageActivity> message, WitResult witResult)
        {
            return Task.CompletedTask;
        }
    }
}