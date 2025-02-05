 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class NetworkService
    {
        [JsonProperty("svc_id")]
        public long Id { get; set; }

        [JsonProperty("svc_name")]
        public string Name { get; set; } = "";

        [JsonProperty("svc_uid")]
        public string Uid { get; set; } = "";

        [JsonProperty("svc_port")]
        public int? DestinationPort { get; set; }

        [JsonProperty("svc_port_end")]
        public int? DestinationPortEnd { get; set; }

        [JsonProperty("svc_source_port")]
        public int? SourcePort { get; set; }

        [JsonProperty("svc_source_port_end")]
        public int? SourcePortEnd { get; set; }

        [JsonProperty("svc_code")]
        public string Code { get; set; } = "";

        [JsonProperty("svc_timeout")]
        public int? Timeout { get; set; }

        [JsonProperty("svc_typ_id")]
        public int? TypeId { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("svc_create")]
        public int Create { get; set; }

        [JsonProperty("svc_create_time")]
        public TimeWrapper CreateTime { get; set; } = new();

        [JsonProperty("svc_last_seen")]
        public int LastSeen { get; set; }

        [JsonProperty("service_type")]
        public NetworkServiceType Type { get; set; } = new();

        [JsonProperty("svc_comment")]
        public string Comment { get; set; } = "";

        [JsonProperty("svc_color_id")]
        public int? ColorId { get; set; }

        [JsonProperty("ip_proto_id")]
        public int? ProtoId { get; set; }

        [JsonProperty("protocol_name")]
        public NetworkProtocol Protocol { get; set; } = new();

        [JsonProperty("svc_member_names")]
        public string MemberNames { get; set; } = "";

        [JsonProperty("svc_member_refs")]
        public string MemberRefs { get; set; } = "";

        [JsonProperty("svcgrps")]
        public Group<NetworkService>[] ServiceGroups { get; set; } = [];

        [JsonProperty("svcgrp_flats")]
        public GroupFlat<NetworkService>[] ServiceGroupFlats { get; set; } = [];

        public long Number;

        public override bool Equals(object? obj)
        {
            return obj switch
            {
                NetworkService nsrv => Id == nsrv.Id,
                _ => base.Equals(obj),
            };
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public string MemberNamesAsHtml()
        {
            if (MemberNames != null && MemberNames.Contains("|"))
            {
                return $"<td>{string.Join("<br>", MemberNames.Split('|'))}</td>";
            }
            else
            {
                return $"<td>{MemberNames}</td>";
            }
        }
    }
}
