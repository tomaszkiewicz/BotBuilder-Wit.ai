using Wit.Bot.Framework.Builder.Attributes;

namespace Wit.Bot.Framework.Builder.Handlers
{
    public class ActionActivityHandlerInfo
    {
        public ActionActivityHandler Handler { get; set; }
        public WitEntityAttribute[] WitEntities { get; set; }
        public bool WitReset { get; set; }
        public WitMerge WitMerges { get; set; }
    }
}