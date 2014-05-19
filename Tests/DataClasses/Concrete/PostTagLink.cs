using System.ComponentModel.DataAnnotations.Schema;

namespace Tests.DataClasses.Concrete
{
    public class PostTagLink
    {

        public int PostTagLinkId { get; set; }

        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public Post InPost { get; set; }

        public int TagId { get; set; }
        [ForeignKey("TagId")]
        public virtual Tag HasTag { get; set; }

        public override string ToString()
        {
            return string.Format("PostTagLinkId: {0}, PostId: {1}, InPost: {2}, TagId: {3}, HasTag: {4}", 
                PostTagLinkId, PostId, InPost == null ? "null" : InPost.Title, TagId, HasTag == null ? "null" : HasTag.Name);
        }
    }
}
