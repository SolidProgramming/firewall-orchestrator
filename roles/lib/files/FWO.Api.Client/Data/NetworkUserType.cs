 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class NetworkUserType
    {
        [JsonProperty("usr_typ_name")]
        public string Name { get; set; } = "";
    }
}
