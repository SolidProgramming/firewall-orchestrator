 
using Newtonsoft.Json;

namespace FWO.Api.Data
{

//  refresh_view_rule_with_owner {
//     id
//     view_name
//     refreshed_at
//     status
//   }
    public class OwnerRefresh
    {

        [JsonProperty("id")]
        private int Id { get; set; } = 0;

        [JsonProperty("view_name")]
        private string ViewName { get; set; } = "";

        [JsonProperty("refreshed_at")]
        private string RefreshedAt { get; set; }

        [JsonProperty("status")]
        private string Status { get; set; } = "";
        

        public string GetStatus()
        {
            return Status;
        }
    }
}
