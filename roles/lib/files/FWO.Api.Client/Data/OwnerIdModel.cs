using Newtonsoft.Json;


namespace FWO.Api.Client.Data
{
    public class OwnerIdModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
