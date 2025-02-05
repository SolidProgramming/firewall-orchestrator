 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class WfComment : WfCommentBase
    {
        [JsonProperty("id")]
        public int Id { get; set; }


        public WfComment()
        { }

        public WfComment(WfComment comment) : base(comment)
        {
            Id = comment.Id;
        }
    }

    public class WfCommentDataHelper
    {
        [JsonProperty("comment")]
        public WfComment Comment { get; set; } = new ();


        public WfCommentDataHelper()
        {}

        public WfCommentDataHelper(WfComment comment)
        {
            Comment = comment;
        }
    }
}
