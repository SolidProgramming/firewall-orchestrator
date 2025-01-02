﻿using System.Text.Json.Serialization; 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class Rule
    {
        [JsonProperty("rule_id")]
        public long Id { get; set; }

        [JsonProperty("rule_uid")]
        public string? Uid { get; set; } = "";

        [JsonProperty("mgm_id")]
        public int MgmtId { get; set; }

        [JsonProperty("rule_num_numeric")]
        public double OrderNumber { get; set; }

        [JsonProperty("rule_name")]
        public string? Name { get; set; } = "";

        [JsonProperty("rule_comment")]
        public string? Comment { get; set; } = "";

        [JsonProperty("rule_disabled")]
        public bool Disabled { get; set; }

        [JsonProperty("rule_services")]
        public ServiceWrapper[] Services { get; set; } = [];

        [JsonProperty("rule_svc_neg")]
        public bool ServiceNegated { get; set; }

        [JsonProperty("rule_svc")]
        public string Service { get; set; } = "";

        [JsonProperty("rule_src_neg")]
        public bool SourceNegated { get; set; }

        [JsonProperty("rule_src")]
        public string Source { get; set; } = "";

        [JsonProperty("src_zone")]
        public NetworkZone? SourceZone { get; set; } = new ();

        [JsonProperty("rule_froms")]
        public NetworkLocation[] Froms { get; set; } = [];
      
        [JsonProperty("rule_dst_neg")]
        public bool DestinationNegated { get; set; }

        [JsonProperty("rule_dst")]
        public string Destination { get; set; } = "";

        [JsonProperty("dst_zone")]
        public NetworkZone? DestinationZone { get; set; } = new ();

        [JsonProperty("rule_tos")]
        public NetworkLocation[] Tos { get; set; } = [];

        [JsonProperty("rule_action")]
        public string Action { get; set; } = "";

        [JsonProperty("rule_track")]
        public string Track { get; set; } = "";

        [JsonProperty("section_header")]
        public string? SectionHeader { get; set; } = "";

        [JsonProperty("rule_metadatum")]
        public RuleMetadata Metadata {get; set;} = new ();

        [JsonProperty("translate")]
        public NatData NatData {get; set;} = new ();

        [JsonProperty("owner_name")]
        public string OwnerName {get; set;} = "";

        [JsonProperty("owner_id")]
        public int? OwnerId {get; set;}

        [JsonProperty("matches")]
        public string IpMatch {get; set;} = "";

        [JsonProperty("dev_id")]
        public int DeviceId { get; set; }

        [JsonProperty("rule_custom_fields")]
        public string CustomFields { get; set; } = "";


        public int DisplayOrderNumber { get; set; }
        public bool Certified { get; set; }
        public string DeviceName { get; set; } = "";
        public NetworkLocation[] DisregardedFroms { get; set; } = [];
        public NetworkLocation[] DisregardedTos { get; set; } = [];
        public bool ShowDisregarded { get; set; } = false;

        public bool IsDropRule()
        {
            return Action == "drop" || Action == "reject" || Action == "deny";
        }

    }
}
