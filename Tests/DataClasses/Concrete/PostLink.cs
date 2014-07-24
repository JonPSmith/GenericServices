using System.ComponentModel.DataAnnotations.Schema;

namespace Tests.DataClasses.Concrete
{
    /// <summary>
    /// This class is purely used to test the SqlException thrown when we try to delete a Post 
    /// that has a PostLink entry that points to the Post 
    /// </summary>
    public class PostLink
    {

        public int PostLinkId { get; set; }

        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public Post PostPart { get; set; }
    }
}
