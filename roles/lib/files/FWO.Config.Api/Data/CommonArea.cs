using Newtonsoft.Json;

using FWO.Basics;
using FWO.Api.Data;

namespace FWO.Config.Api.Data
{
    public class CommonAreaConfig
    {
        [JsonProperty("area_id")]
        public long AreaId { get; set; } = 0;

        [JsonProperty("use_in_src")]
        public bool UseInSrc { get; set; } = true;

        [JsonProperty("use_in_dst")]
        public bool UseInDst { get; set; } = true;
    }

    public class CommonArea
    {
        public ModellingNwGroupWrapper Area { get; set; } = new();

        public bool UseInSrc { get; set; } = true;

        public bool UseInDst { get; set; } = true;

        public CommonAreaConfig ToConfigItem()
        {
            return new(){ AreaId = Area.Content.Id, UseInSrc = UseInSrc, UseInDst = UseInDst};
        }
    }
}
