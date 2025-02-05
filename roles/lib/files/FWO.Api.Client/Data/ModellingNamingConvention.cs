 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class ModellingNamingConvention
    {
        [JsonProperty("networkAreaRequired")]
        public bool NetworkAreaRequired { get; set; } = false;

        [JsonProperty("useAppPart")]
        public bool UseAppPart { get; set; } = false;

        [JsonProperty("fixedPartLength")]
        public int FixedPartLength { get; set; }

        [JsonProperty("freePartLength")]
        public int FreePartLength { get; set; }

        [JsonProperty("networkAreaPattern")]
        public string NetworkAreaPattern { get; set; } = "";

        [JsonProperty("appRolePattern")]
        public string AppRolePattern { get; set; } = "";

        [JsonProperty("applicationZone")]
        public string AppZone { get; set; } = "";
        
        [JsonProperty("appServerPrefix")]
        public string? AppServerPrefix { get; set; } = "";
    }
}
