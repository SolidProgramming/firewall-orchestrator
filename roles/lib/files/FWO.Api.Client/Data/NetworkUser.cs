using System.Text.Json.Serialization; 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class NetworkUser
    {
        [JsonProperty("user_id")]
        public long Id { get; set; }

        [JsonProperty("user_uid")]
        public string Uid { get; set; } = "";

        [JsonProperty("user_name")]
        public string Name { get; set; } = "";

        [JsonProperty("user_comment")]
        public string Comment { get; set; } = "";

        [JsonProperty("user_lastname")]
        public string LastName { get; set; } = "";

        [JsonProperty("user_firstname")]
        public string FirstName { get; set; } = "";

        [JsonProperty("usr_typ_id")]
        public int TypeId { get; set; }

        [JsonProperty("type")]
        public NetworkUserType Type { get; set; } = new(){};

        [JsonProperty("user_create")]
        public int Create { get; set; }

        [JsonProperty("user_create_time")]
        public TimeWrapper CreateTime { get; set; } = new(){};

        [JsonProperty("user_last_seen")]
        public int LastSeen { get; set; }

        [JsonProperty("user_member_names")]
        public string MemberNames { get; set; } = "";

        [JsonProperty("user_member_refs")]
        public string MemberRefs { get; set; } = "";

        [JsonProperty("usergrps")]
        public Group<NetworkUser>[] UserGroups { get; set; } = new Group<NetworkUser>[]{};

        [JsonProperty("usergrp_flats")]
        public GroupFlat<NetworkUser>[] UserGroupFlats { get; set; } = new GroupFlat<NetworkUser>[]{};

        public override bool Equals(object? obj)
        {
            return obj switch
            {
                NetworkUser user => Id == user.Id,
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
