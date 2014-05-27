using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using GenericServices;
using GenericServices.Services;
using Tests.DataClasses.Concrete;

namespace Tests.DTOs.Concrete
{
    public class SimpleBlogWithPostsDto : InstrumentedEfGenericDto<Blog, SimpleBlogWithPostsDto>, ISimpleBlogWithPostsDto
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

        public ICollection<Post> Posts { get; protected set; }

        protected internal override ServiceFunctions SupportedFunctions
        {
            get { return ServiceFunctions.List | ServiceFunctions.DoesNotNeedSetup; }
        }

        protected override IQueryable<Blog> GetDataUntracked(IDbContextWithValidation context)
        {
            return base.GetDataUntracked(context).Include(x => x.Posts);
        }
    }
}
