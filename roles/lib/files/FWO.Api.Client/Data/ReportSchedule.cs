 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class ReportSchedule
    {
        [JsonProperty("report_schedule_id")]
        public int Id { get; set; }

        [JsonProperty("report_schedule_name")]
        public string Name { get; set; } = "";

        [JsonProperty("report_schedule_owner_user")]
        public UiUser ScheduleOwningUser { get; set; } = new ();

        [JsonProperty("report_schedule_start_time")]
        public DateTime StartTime { get; set; } = DateTime.Now.AddSeconds(-DateTime.Now.Second);

        [JsonProperty("report_schedule_repeat")]
        public int RepeatOffset { get; set; } = 1;

        [JsonProperty("report_schedule_every")]
        public Interval RepeatInterval { get; set; }

        [JsonProperty("report_schedule_template")]
        public ReportTemplate Template { get; set; } = new ();

        [JsonProperty("report_schedule_formats")]
        public List<FileFormat> OutputFormat { get; set; } = [];

        [JsonProperty("report_schedule_active")]
        public bool Active { get; set; } = true;

        [JsonProperty("report_schedule_counter")]
        public int Counter { get; set; }

        public bool Sanitize()
        {
            bool shortened = false;
            Name = Sanitizer.SanitizeMand(Name, ref shortened);
            return shortened;
        }
    }

    public enum Interval
    {
        Never = 0,
        Days = 2,
        Weeks = 3,
        Months = 4,
        Years = 5
    }
}
