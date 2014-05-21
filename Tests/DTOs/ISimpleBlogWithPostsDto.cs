using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tests.DataClasses.Concrete;

namespace Tests.DTOs
{
    public interface ISimpleBlogWithPostsDto
    {
        [Key]
        int BlogId { get; set; }

        [MinLength(2)]
        [MaxLength(64)]
        [Required]
        string Name { get; set; }

        [MaxLength(256)]
        [Required]
        string EmailAddress { get; set; }

        List<Post> Posts { get; }
    }
}