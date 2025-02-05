 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class RuleChange
    {
        [JsonProperty("import")]
        public ChangeImport ChangeImport { get; set; } = new ChangeImport();

        [JsonProperty("change_action")]
        public char ChangeAction { get; set; }

        [JsonProperty("old")]
        public Rule OldRule { get; set; } = new Rule();

        [JsonProperty("new")]
        public Rule NewRule { get; set; } = new Rule();

        public string DeviceName { get; set; } = "";
    }
}
