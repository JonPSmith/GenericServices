using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tests.DataClasses.Concrete
{
    public class Blog
    {
        public int BlogId { get; set; }
        [MinLength(2)]
        [MaxLength(64)]
        [Required]
        public string Name { get; set; }

        [MaxLength(256)]
        [Required]
        public string EmailAddress { get; set; }

        public ICollection<Post> Posts { get; set; }

        public override string ToString()
        {
            return string.Format("BlogId: {0}, Name: {1}, EmailAddress: {2}, NumPosts: {3}", 
                BlogId, Name, EmailAddress, Posts == null ? "null" : Posts.Count.ToString());
        }
    } 
}
