using Tomaszkiewicz.WitAi.Attributes;

namespace Tomaszkiewicz.WitAi.Handlers
{
    public class ActionHandlerInfo
    {
        public ActionHandler Handler { get; set; }
        public WitRequireEntityAttribute[] WitRequireEntities { get; set; }
        public bool WitReset { get; set; }
        public bool MergeAll { get; set; }
        public WitMergeAttribute[] WitMerges { get; set; }
        public WitLoadPrivateConversationData[] WitLoadPrivateConversationData { get; set; }
    }
}