 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class WfApproval : WfApprovalBase
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("task_id")]
        public long TaskId { get; set; }

        [JsonProperty("comments")]
        public List<WfCommentDataHelper> Comments { get; set; } = [];


        public WfApproval()
        { }

        public WfApproval(WfApproval approval) : base(approval)
        {
            Id = approval.Id;
            TaskId = approval.TaskId;
            Comments = approval.Comments;
        }
    }

    public class ApprovalParams
    {
        [JsonProperty("state_id")]
        public int StateId { get; set; }

        [JsonProperty("approver_group")]
        public string ApproverGroup { get; set; } = "";

        [JsonProperty("deadline")]
        public int Deadline { get; set; }
    }
}
