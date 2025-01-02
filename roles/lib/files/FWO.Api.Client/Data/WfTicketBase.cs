using System.Text.Json.Serialization; 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class WfTicketBase: WfStatefulObject
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; } = "";

        [JsonProperty("date_created")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("date_completed")]
        public DateTime? CompletionDate { get; set; }

        [JsonProperty("requester")]
        public UiUser? Requester { get; set; }

        [JsonProperty("requester_dn")]
        public string? RequesterDn { get; set; } = "";

        [JsonProperty("requester_group")]
        public string? RequesterGroup { get; set; }

        [JsonProperty("tenant_id")]
        public int? TenantId { get; set; }

        [JsonProperty("reason")]
        public string? Reason { get; set; }

        [JsonProperty("external_ticket_id")]
        public string? ExternalTicketId { get; set; }

        [JsonProperty("external_ticket_source")]
        public int? ExternalTicketSource { get; set; }

        [JsonProperty("ticket_deadline")]
        public DateTime? Deadline { get; set; }

        [JsonProperty("ticket_priority")]
        public int? Priority { get; set; }


        public WfTicketBase()
        { }

        public WfTicketBase(WfTicketBase ticket) : base(ticket)
        {
            Id = ticket.Id;
            Title = ticket.Title;
            CreationDate = ticket.CreationDate;
            CompletionDate = ticket.CompletionDate;
            Requester = ticket.Requester;
            RequesterDn = ticket.RequesterDn;
            RequesterGroup = ticket.RequesterGroup;
            TenantId = ticket.TenantId;
            Reason = ticket.Reason;
            ExternalTicketId = ticket.ExternalTicketId;
            ExternalTicketSource = ticket.ExternalTicketSource;
            Deadline = ticket.Deadline;
        }

        public override bool Sanitize()
        {
            bool shortened = base.Sanitize();
            Title = Sanitizer.SanitizeMand(Title, ref shortened);
            RequesterDn = Sanitizer.SanitizeLdapPathOpt(RequesterDn, ref shortened);
            RequesterGroup = Sanitizer.SanitizeLdapPathOpt(RequesterGroup, ref shortened);
            Reason = Sanitizer.SanitizeOpt(Reason, ref shortened);
            ExternalTicketId = Sanitizer.SanitizeOpt(ExternalTicketId, ref shortened);
            return shortened;
        }
    }
}
