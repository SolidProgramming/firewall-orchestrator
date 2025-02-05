 
using Newtonsoft.Json;


namespace FWO.Api.Data
{
    public class WfExtState
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("state_id")]
        public int? StateId { get; set; }
    }
}
