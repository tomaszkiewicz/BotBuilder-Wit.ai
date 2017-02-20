using System;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;

namespace Wit.Bot.Framework.Builder.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    [Serializable]
    public class WitActionAttribute : AttributeString
    {
        public readonly string ActionName;

        public WitActionAttribute(string actionName)
        {
            SetField.NotNull(out ActionName, nameof(actionName), actionName);
        }

        protected override string Text => ActionName;
    }
}