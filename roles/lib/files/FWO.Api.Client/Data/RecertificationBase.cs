
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class RecertificationBase
    {

        [JsonProperty("recert_date")]
        public DateTime? RecertDate { get; set; }

        [JsonProperty("recertified")]
        public bool Recertified { get; set; } = false;

        [JsonProperty("ip_match")]
        public string IpMatch { get; set; } = "";

        [JsonProperty("next_recert_date")]
        public DateTime? NextRecertDate { get; set; }

        [JsonProperty("owner_id")]
        public int OwnerId { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; } = "";

        [JsonProperty("rule_id")]
        public int RuleId { get; set; }

        [JsonProperty("rule_metadata_id")]
        public int RuleMetadataId { get; set; }
    }

}
