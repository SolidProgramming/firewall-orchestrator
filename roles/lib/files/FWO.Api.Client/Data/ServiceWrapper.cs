 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class ServiceWrapper
    {
        [JsonProperty("service")]
        public NetworkService Content { get; set; } = new NetworkService();
    }
    // public class ServiceObjectRecursiveWrapper
    // {
    //     [JsonProperty("service")]
    //     public ServiceObjectRecursiveFlatsWrapper Content { get; set; }
    // }
}
