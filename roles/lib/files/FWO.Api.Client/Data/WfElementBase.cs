 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public enum ElemFieldType
    {
        source, 
        destination, 
        service,
        rule
    }

    public class WfElementBase
    {
        [JsonProperty("ip")]
        public string? IpString { get; set; }

        [JsonProperty("ip_end")]
        public string? IpEnd { get; set; }

        [JsonProperty("port")]
        public int? Port { get; set; }

        [JsonProperty("port_end")]
        public int? PortEnd { get; set; }

        [JsonProperty("ip_proto_id")]
        public int? ProtoId { get; set; }

        [JsonProperty("network_object_id")]
        public long? NetworkId { get; set; }

        [JsonProperty("service_id")]
        public long? ServiceId { get; set; }

        [JsonProperty("field")]
        public string Field { get; set; } = ElemFieldType.source.ToString();

        [JsonProperty("user_id")]
        public long? UserId { get; set; }

        [JsonProperty("original_nat_id")]
        public long? OriginalNatId { get; set; }

        [JsonProperty("rule_uid")]
        public string? RuleUid { get; set; }

        [JsonProperty("group_name")]
        public string? GroupName { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }


        public WfElementBase()
        { }

        public WfElementBase(WfElementBase element)
        {
            IpString = element.IpString;
            IpEnd = element.IpEnd;
            Port = element.Port;
            PortEnd = element.PortEnd;
            ProtoId = element.ProtoId;
            NetworkId = element.NetworkId;
            ServiceId = element.ServiceId;
            Field = element.Field;
            UserId = element.UserId;
            OriginalNatId = element.OriginalNatId;
            RuleUid = element.RuleUid;
            GroupName = element.GroupName;
            Name = element.Name;
        }

        public static NetworkObject ToNetworkObject(WfElementBase elem)
        {
            return new NetworkObject()
            {
                Name = elem.Name ?? "",
                IP = elem.IpString ?? "",
                IpEnd = elem.IpEnd ?? ""
            };
        }

        public virtual bool Sanitize()
        {
            bool shortened = false;
            IpString = Sanitizer.SanitizeOpt(IpString, ref shortened);
            IpEnd = Sanitizer.SanitizeOpt(IpEnd, ref shortened);
            Field = Sanitizer.SanitizeMand(Field, ref shortened);
            RuleUid = Sanitizer.SanitizeOpt(RuleUid, ref shortened);
            GroupName = Sanitizer.SanitizeOpt(GroupName, ref shortened);
            Name = Sanitizer.SanitizeOpt(Name, ref shortened);
            return shortened;
        }
    }
}
