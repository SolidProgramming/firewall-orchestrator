 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class NetworkObjectType
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";
    }
}
