using System.Text.Json.Serialization; 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class Group<T>
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("byId")]
        public T? Object { get; set; }
    }
}
