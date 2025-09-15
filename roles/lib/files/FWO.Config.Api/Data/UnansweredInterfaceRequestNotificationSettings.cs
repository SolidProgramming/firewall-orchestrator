using FWO.Data;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace FWO.Config.Api.Data
{
    public class UnansweredInterfaceRequestNotificationSettings
    {
        [JsonProperty("unanswered_interface_notifications_active"), JsonPropertyName("unanswered_interface_notifications_active")]
        public bool Active { get; set; }

        [JsonProperty("unanswered_interface_notifications_time"), JsonPropertyName("unanswered_interface_notifications_time")]        
        public TimeOnly StartTime { get; set; } = TimeOnly.MinValue;

        [JsonProperty("unanswered_interface_notifications_date"), JsonPropertyName("unanswered_interface_notifications_date")]
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [JsonProperty("unanswered_interface_notifications_interval"), JsonPropertyName("unanswered_interface_notifications_interval")]
        public int Interval { get; set; } = 1;

        [JsonProperty("unanswered_interface_notifications_schedulerinterval"), JsonPropertyName("unanswered_interface_notifications_schedulerinterval")]
        public SchedulerInterval SchedulerInterval { get; set; } = SchedulerInterval.Weeks;

        [JsonProperty("unanswered_interface_notifications_notificationmaximum"), JsonPropertyName("unanswered_interface_notifications_notificationmaximum")]
        public int NotificationsMaximum { get; set; } = 0;
    }
}
