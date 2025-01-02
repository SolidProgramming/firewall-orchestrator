using System.Text.Json.Serialization; 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class ReportTemplate
    {
        [JsonProperty("report_template_id")]
        public int Id { get; set; }

        [JsonProperty("report_template_name")]
        public string Name { get; set; } = "";

        [JsonProperty("report_template_create")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("report_template_comment")]
        public string Comment { get; set; } = "";

        [JsonProperty("report_template_owner")]
        public int TemplateOwningUserId { get; set; }

        [JsonProperty("report_filter")]
        public string Filter { get; set; } = "";
        
        [JsonProperty("report_parameters")]
        public ReportParams ReportParams { get; set; } = new();

        public bool Detailed = false;

   
        public ReportTemplate()
        {}

        public ReportTemplate(string filter, ReportParams reportParams)
        {
            Filter = filter;
            ReportParams = reportParams;
            Detailed = false;
        }

        public bool Sanitize()
        {
            bool shortened = false;
            Name = Sanitizer.SanitizeMand(Name, ref shortened);
            Comment = Sanitizer.SanitizeMand(Comment, ref shortened);
            return shortened;
        }
    }

    public class ReportParams
    {
        [JsonProperty("report_type")]
        public int ReportType { get; set; } = 0;
        
        [JsonProperty("device_filter")]
        public DeviceFilter DeviceFilter { get; set; } = new();

        [JsonProperty("time_filter")]
        public TimeFilter TimeFilter { get; set; } = new();

        [JsonProperty("tenant_filter")]
        public TenantFilter TenantFilter { get; set; } = new();

        [JsonProperty("recert_filter")]
        public RecertFilter RecertFilter { get; set; } = new();

        [JsonProperty("unused_filter")]
        public UnusedFilter UnusedFilter { get; set; } = new();

        [JsonProperty("modelling_filter")]
        public ModellingFilter ModellingFilter { get; set; } = new();

        public ReportParams()
        {}
        
        public ReportParams(int reportType, DeviceFilter deviceFilter)
        {
            ReportType = reportType;
            DeviceFilter = deviceFilter;
        }
    }
}
