using System.Text.Json.Serialization; 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class TicketId
    {
        [JsonProperty("ticket_id")]
        public long Id { get; set; }
    }
}
