using Tomaszkiewicz.WitAi.Interfaces;

namespace Tomaszkiewicz.WitAi.Console
{
    internal class ThanksHandler : ConsoleHandler, IIntentHandler
    {
        public string Intent => "thanks";
    }
}