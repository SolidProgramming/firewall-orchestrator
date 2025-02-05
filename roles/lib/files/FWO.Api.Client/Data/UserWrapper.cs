 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class UserWrapper
    {
        [JsonProperty("usr")]
        public NetworkUser Content { get; set; } = new NetworkUser();
    }
}
