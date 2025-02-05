 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class WfApprovalBase : WfStatefulObject
    {
        [JsonProperty("date_opened")]
        public DateTime DateOpened { get; set; } = DateTime.Now;

        [JsonProperty("approval_date")]
        public DateTime? ApprovalDate { get; set; }

        [JsonProperty("approval_deadline")]
        public DateTime? Deadline { get; set; }

        [JsonProperty("approver_group")]
        public string? ApproverGroup { get; set; }

//        [JsonProperty("approver")]
//        public UiUser? Approver { get; set; }

        [JsonProperty("approver")]
        public string? ApproverDn { get; set; } = "";

        [JsonProperty("tenant_id")]
        public int? TenantId { get; set; }

        [JsonProperty("initial_approval")]
        public bool InitialApproval { get; set; } = true;


        public WfApprovalBase()
        { }

        public WfApprovalBase(WfApprovalBase approval) : base(approval)
        {
            DateOpened = approval.DateOpened;
            ApprovalDate = approval.ApprovalDate;
            Deadline = approval.Deadline;
            ApproverGroup = approval.ApproverGroup;
            ApproverDn = approval.ApproverDn;
            TenantId = approval.TenantId;
            InitialApproval = approval.InitialApproval;
         }

        public override bool Sanitize()
        {
            bool shortened = base.Sanitize();
            ApproverGroup = Sanitizer.SanitizeLdapPathOpt(ApproverGroup, ref shortened);
            ApproverDn = Sanitizer.SanitizeLdapPathOpt(ApproverDn, ref shortened);
            return shortened;
        }
    }
}
