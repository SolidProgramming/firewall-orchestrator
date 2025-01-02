using System.Text.Json.Serialization; 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class NetworkProtocol
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = "";


        public NetworkProtocol()
        {}

        public NetworkProtocol(IpProtocol i)
        {
            Id = i.Id;
            Name = i.Name;
        }
    }
}
