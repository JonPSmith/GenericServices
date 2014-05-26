using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tests.DataClasses.Concrete
{
    public class PostTagGrade
    {
        [Key]
        [Column(Order = 1)] 
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public Post PostPart { get; set; }

        [Key]
        [Column(Order = 2)] 
        public int TagId { get; set; }
        [ForeignKey("TagId")]
        public Tag TagPart { get; set; }

        public int Grade { get; set; }
    }
}
