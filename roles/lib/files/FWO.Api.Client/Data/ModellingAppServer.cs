using System.Text.Json.Serialization; 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class ModellingAppServer

    {
        [JsonProperty("id"), JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonProperty("name"), JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonProperty("app_id"), JsonPropertyName("app_id")]
        public int AppId { get; set; }

        [JsonProperty("ip"), JsonPropertyName("ip")]
        public string Ip { get; set; } = "";

        [JsonProperty("import_source"), JsonPropertyName("import_source")]
        public string ImportSource { get; set; } = "";

        [JsonProperty("is_deleted"), JsonPropertyName("is_deleted")]
        public bool IsDeleted { get; set; }

        public string ExtAppId { get; set; } = "";
        
        public static NetworkObject ToNetworkObject(ModellingAppServer appServer)
        {
            return new NetworkObject()
            {
                Id = appServer.Id,
                Name = appServer.Name,
                IP = appServer.Ip,
                IpEnd = appServer.Ip
            };
        }
    }

    public class ModellingAppServerWrapper
    {
        [JsonProperty("app_server"), JsonPropertyName("app_server")]
        public ModellingAppServer Content { get; set; } = new();

        public static ModellingAppServer[] Resolve(List<ModellingAppServerWrapper> wrappedList)
        {
            return Array.ConvertAll(wrappedList.ToArray(), wrapper => wrapper.Content);
        }
    }
}
