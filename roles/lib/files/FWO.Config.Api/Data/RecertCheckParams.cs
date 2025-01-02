using Newtonsoft.Json;
using System.Text.Json.Serialization;
using FWO.Basics;
using FWO.Api.Data;

namespace FWO.Config.Api.Data
{
    public class RecertCheckParams
    {
        [JsonProperty("check_interval")]
        public Interval RecertCheckInterval { get; set; } = Interval.Months;

        [JsonProperty("check_offset")]
        public int RecertCheckOffset { get; set; } = 1;

        [JsonProperty("check_weekday")]
        public int? RecertCheckWeekday { get; set; }

        [JsonProperty("check_dayofmonth")]
        public int? RecertCheckDayOfMonth { get; set; }
    }
}
