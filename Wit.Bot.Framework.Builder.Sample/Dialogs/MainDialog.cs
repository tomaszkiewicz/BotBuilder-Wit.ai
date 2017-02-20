using System;
using System.Linq;
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

        public void Merge(WitResult result)
        {
            if (result.Entities == null)
                return;

            foreach (var entity in result.Entities.Where(e => e.Key != "intent"))
                WitContext[entity.Key] = entity.Value.FirstOrDefault()?.Value;
        }

        public bool ValidateWitContextKey(string key, string errorKey)
        {
            if (!WitContext.ContainsKey(key))
            {
                WitContext[errorKey] = true;

                return false;
            }

            WitContext.Remove(errorKey);

            return true;
        }

        [WitAction("getForecast")]
        [WitEntity("location")]
        [WitEntity("datetime")]
        [WitMerge]
        public async Task GetForecast(IDialogContext context, IAwaitable<IMessageActivity> message, WitResult witResult)
        {
            Merge(witResult);

            if (!ValidateWitContextKey("location", "missingLocation"))
                return;

            if (!ValidateWitContextKey("datetime", "missingDatetime"))
                return;

            WitContext["forecast"] = "Sunny";
        }

        [WitAction("reset")]
        [WitReset]
        public Task Reset(IDialogContext context, IAwaitable<IMessageActivity> message, WitResult witResult)
        {
            RequestReset();

            return Task.CompletedTask;
        }
    }
}