 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public enum TimeRangeType
    {
        Shortcut = 0,
        Interval = 1,
        Fixeddates = 2
    }

    public class TimeRangeShortcuts
    {
        // of course an enum would be better, but there are already values with blanks in the database
        public static List<string> Ranges = new List<string>
        {
            "this year",
            "last year",
            "this month",
            "last month",
            "this week",
            "last week",
            "today",
            "yesterday"
        };
    }

    public class TimeFilter
    {
        [JsonProperty("is_shortcut")]
        public bool IsShortcut { get; set; } = true;

        [JsonProperty("shortcut")]
        public string TimeShortcut { get; set; } = "now";

        [JsonProperty("report_time")]
        public DateTime ReportTime { get; set; } = DateTime.Now.AddSeconds(-DateTime.Now.Second);


        [JsonProperty("timerange_type")]
        public TimeRangeType TimeRangeType { get; set; } = TimeRangeType.Shortcut;

        [JsonProperty("shortcut_range")]
        public string TimeRangeShortcut { get; set; } = "this year";

        [JsonProperty("offset")]
        public int Offset { get; set; } = 0;

        [JsonProperty("interval")]
        public Interval Interval { get; set; } = Interval.Days;

        [JsonProperty("start_time")]
        public DateTime StartTime { get; set; } = DateTime.Now.AddSeconds(-DateTime.Now.Second);

        [JsonProperty("open_start")]
        public bool OpenStart { get; set; } = false;

        [JsonProperty("end_time")]
        public DateTime EndTime { get; set; } = DateTime.Now.AddSeconds(-DateTime.Now.Second);

        [JsonProperty("open_end")]
        public bool OpenEnd { get; set; } = false;
    }
}
