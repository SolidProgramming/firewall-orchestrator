using System.Text.Json.Serialization; 
using Newtonsoft.Json;


namespace FWO.Api.Data
{
    public class WfPriority
    {
        [JsonProperty("numeric_prio")]
        public int NumPrio { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("ticket_deadline")]
        public int TicketDeadline { get; set; }

        [JsonProperty("approval_deadline")]
        public int ApprovalDeadline { get; set; }
    }
}
