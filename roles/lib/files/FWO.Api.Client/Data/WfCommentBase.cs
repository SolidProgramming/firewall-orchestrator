using System.Text.Json.Serialization; 
using Newtonsoft.Json;

namespace FWO.Api.Data
{
    public class WfCommentBase
    {
        [JsonProperty("ref_id")]
        public long? RefId { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; } = "";

        [JsonProperty("creation_date")]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        [JsonProperty("creator")]
        public UiUser Creator { get; set; } = new UiUser();

        [JsonProperty("comment_text")]
        public string CommentText { get; set; } = "";


        public WfCommentBase()
        { }

        public WfCommentBase(WfCommentBase comment)
        {
            RefId = comment.RefId;
            Scope = comment.Scope;
            CreationDate = comment.CreationDate;
            Creator = comment.Creator;
            CommentText = comment.CommentText;
        }

        public virtual bool Sanitize()
        {
            bool shortened = false;
            Scope = Sanitizer.SanitizeMand(Scope, ref shortened);
            CommentText = Sanitizer.SanitizeMand(CommentText, ref shortened);
            return shortened;
        }
    }
}
