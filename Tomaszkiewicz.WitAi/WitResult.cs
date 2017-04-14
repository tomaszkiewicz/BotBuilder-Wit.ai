using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tomaszkiewicz.WitAi
{
    public class WitResult
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }

        [JsonProperty("confidence")]
        public float Confidence { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("quickreplies")]
        public string[] QuickReplies { get; set; }

        [JsonProperty("entities")]
        public Dictionary<string, IList<WitEntity>> Entities { get; set; }
    }
}
