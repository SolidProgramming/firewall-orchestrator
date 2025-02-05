using Newtonsoft.Json; 

namespace FWO.Api.Client
{
    public class ReturnId
    {
        [JsonProperty("newId")]
        public int NewId { get; set; }

        [JsonProperty("UpdatedId")]
        public int UpdatedId { get; set; }

        [JsonProperty("DeletedId")]
        public int DeletedId { get; set; }

        [JsonProperty("affected_rows")]
        public int AffectedRows { get; set; }

        [JsonProperty("uiuser_password_must_be_changed")]
        public bool PasswordMustBeChanged { get; set; }
    }
    
    public class NewReturning
    {
        [JsonProperty("returning")]
        public ReturnId[]? ReturnIds { get; set; }
    }

    public class AggregateCount
    {
        [JsonProperty("aggregate")]
        public Aggregate Aggregate {get; set;} = new Aggregate();
    }

    public class Aggregate
    {
        [JsonProperty("count")]     
        public int Count { get; set; }
    }
}
