using Newtonsoft.Json;

namespace Tomaszkiewicz.WitAi
{
    public class WitEntity
    {
        [JsonProperty("confidence")]
        public float Confidence { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("suggested")]
        public bool Suggested { get; set; }
    }
}
