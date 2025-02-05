 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class NetworkObjectWrapper
    {
        [JsonProperty("object")]
        public NetworkObject Content { get; set; } = new NetworkObject(){};
    }
}
