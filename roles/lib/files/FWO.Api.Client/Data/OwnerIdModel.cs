using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace FWO.Api.Client.Data
{
    public class OwnerIdModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
