 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class WfReqElementWriter : WfElementBase
    {
        [JsonProperty("request_action")]
        public string RequestAction { get; set; } = Data.RequestAction.create.ToString();

        [JsonProperty("device_id")]
        public int? DeviceId { get; set; }

        public WfReqElementWriter()
        {}

        public WfReqElementWriter(WfReqElement element) : base(element)
        { 
            RequestAction = element.RequestAction;
            DeviceId = element.DeviceId;
            IpString = element.Cidr != null && element.Cidr.Valid ? element.Cidr.CidrString : null;
            IpEnd = element.CidrEnd != null && element.CidrEnd.Valid ? element.CidrEnd.CidrString : null;
        }
    }
}
