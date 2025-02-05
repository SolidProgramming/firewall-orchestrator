 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class Import
    {
        [JsonProperty("aggregate")]
        public ImportAggregate ImportAggregate { get; set; } = new ImportAggregate();
    }

    public class ImportAggregate
    {
        [JsonProperty("max")]
        public ImportAggregateMax ImportAggregateMax { get; set; } = new ImportAggregateMax();
    }

    public class ImportAggregateMax
    {
        [JsonProperty("id")]
        public long? RelevantImportId { get; set; }
    }

}
