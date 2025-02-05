 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class LogEntry
    {
        [JsonProperty("data_issue_id")]
        public long Id { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; } = "";

        [JsonProperty("severity")]
        public int Severity { get; set; }

        [JsonProperty("issue_timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("suspected_cause")]
        public string? SuspectedCause { get; set; }

        [JsonProperty("issue_mgm_id")]
        public int? ManagementId { get; set; }

        [JsonProperty("issue_dev_id")]
        public int? DeviceId { get; set; }

        [JsonProperty("import_id")]
        public long? ImportId { get; set; }

        [JsonProperty("object_type")]
        public string? ObjectType { get; set; }

        [JsonProperty("object_name")]
        public string? ObjectName { get; set; }

        [JsonProperty("object_uid")]
        public string? ObjectUid { get; set; }

        [JsonProperty("rule_uid")]
        public string? RuleUid { get; set; }

        [JsonProperty("rule_id")]
        public long? RuleId { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("user_id")]
        public int? UserId { get; set; }
    }
}
