using System.Text.Json.Serialization; 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class NetworkServiceType
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";
    }
}
