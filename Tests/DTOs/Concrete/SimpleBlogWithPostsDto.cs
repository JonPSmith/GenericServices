using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using GenericServices;
using GenericServices.Concrete;
using Tests.DataClasses.Concrete;

namespace Tests.DTOs.Concrete
{
    public class SimpleBlogWithPostsDto : EfGenericDto<Blog, SimpleBlogWithPostsDto>
    {
        [Key]
        public int BlogId { get; set; }

        [MinLength(2)]
        [MaxLength(64)]
        [Required]
        public string Name { get; set; }

        [MaxLength(256)]
        [Required]
        public string EmailAddress { get; set; }

        public List<Post> Posts { get; protected set; }

        protected internal override ServiceFunctions SupportedFunctions
        {
            get { return ServiceFunctions.List | ServiceFunctions.DoesNotNeedSetup; }
        }

        protected internal override IQueryable<Blog> GetDataUntracked(IDbContextWithValidation context)
        {
            return base.GetDataUntracked(context).Include(x => x.Posts);
        }
    }
}
