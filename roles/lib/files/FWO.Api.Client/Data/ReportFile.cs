using System.Text.Json.Serialization; 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class ReportFile
    {
        [JsonProperty("report_id")]
        public int Id { get; set; }

        [JsonProperty("report_name")]
        public string Name { get; set; } = "";

        [JsonProperty("report_start_time")]
        public DateTime GenerationDateStart { get; set; }

        [JsonProperty("report_end_time")]
        public DateTime GenerationDateEnd { get; set; }

        [JsonProperty("report_template")]
        public ReportTemplate Template { get; set; } = new ();

        [JsonProperty("report_template_id")]
        public int TemplateId { get; set; }

        [JsonProperty("uiuser")]
        public UiUser ReportOwningUser { get; set; } = new ();

        [JsonProperty("report_owner_id")]
        public int OwnerId { get; set; }

        [JsonProperty("report_json")]
        public string? Json { get; set; }

        [JsonProperty("report_pdf")]
        public string? Pdf { get; set; }

        [JsonProperty("report_html")]
        public string? Html { get; set; }

        [JsonProperty("report_csv")]
        public string? Csv { get; set; }

        [JsonProperty("report_type")]
        public int? Type { get; set; }

        [JsonProperty("description")]
        public String? Description { get; set; }

        public bool Sanitize()
        {
            bool shortened = false;
            Name = Sanitizer.SanitizeMand(Name, ref shortened);
            return shortened;
        }
    }
}
