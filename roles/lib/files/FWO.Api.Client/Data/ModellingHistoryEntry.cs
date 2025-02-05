 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class ModellingHistoryEntry
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("app_id")]
        public int? AppId { get; set; }

        [JsonProperty("change_type")]
        public int ChangeType { get; set; }

        [JsonProperty("object_type")]
        public int ObjectType { get; set; }

        [JsonProperty("object_id")]
        public long ObjectId { get; set; }

        [JsonProperty("change_text")]
        public string ChangeText { get; set; } = "";

        [JsonProperty("changer")]
        public string Changer { get; set; } = "";

        [JsonProperty("change_time")]
        public DateTime? ChangeTime { get; set; }
    }
}
