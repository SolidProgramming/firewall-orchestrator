
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class Recertification : RecertificationBase
    {
        [JsonProperty("owner")]
        public FwoOwner? FwoOwner { get; set; } = new FwoOwner();

        [JsonProperty("user_dn")]
        public string UserDn { get; set; }  = "";
    }
}
