using System.Text.Json.Serialization; 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class GroupFlat<T>
    {
        [JsonProperty("flat_id")]
        public long Id { get; set; }

        [JsonProperty("byFlatId")]
        public T? Object { get; set; }
    }
}
