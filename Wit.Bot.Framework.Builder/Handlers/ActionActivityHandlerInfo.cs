using Wit.Bot.Framework.Builder.Attributes;

namespace Wit.Bot.Framework.Builder.Handlers
{
    public class ActionActivityHandlerInfo
    {
        public ActionActivityHandler Handler { get; set; }
        public WitRequireEntityAttribute[] WitRequireEntities { get; set; }
        public bool WitReset { get; set; }
        public bool MergeAll { get; set; }
        public WitMergeAttribute[] WitMerges { get; set; }
    }
}