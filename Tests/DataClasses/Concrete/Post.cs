using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Tests.DataClasses.Concrete.Helpers;

namespace Tests.DataClasses.Concrete
{

    public class Post : TrackUpdate, IValidatableObject
    {
        public int PostId { get; set; }

        [MinLength(2), MaxLength(128)]
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public int BlogId { get; set; }
        public virtual Blog Blogger { get; set; }

        public ICollection<PostTagLink> AllocatedTags { get; set; }

        public override string ToString()
        {
            return string.Format("PostId: {0}, Title: {1}, BlogId: {2}, Blogger: {3}, AllocatedTags: {4}", 
                PostId, Title, BlogId, Blogger == null ? "null" : Blogger.Name, AllocatedTags == null ? "null" : AllocatedTags.Count().ToString());
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //Note that Tags may be null if the read din't include them, in which case we can't check them
            if (AllocatedTags != null && !AllocatedTags.Any())
                yield return new ValidationResult("The post must have at least one Tag.", new[] { "AllocatedTags" });

            if (Title.Contains("!"))
                yield return new ValidationResult( "Sorry, but you can't get too excited and include a ! in the title.", new [] { "Title"});
            if (Title.EndsWith("?"))
                yield return new ValidationResult("Sorry, but you can't ask a question, i.e. the title can't end with '?'.", new[] { "Title" });

            //These produce top-level errors, i.e. not assocuiated with a property (used to test non-property error reporting)
            if (Content.Contains(" sheep."))
                yield return new ValidationResult("Sorry. Not allowed to end a sentance with 'sheep'.");
            if (Content.Contains(" lamb."))
                yield return new ValidationResult("Sorry. Not allowed to end a sentance with 'lamb'.");
            if (Content.Contains(" cow."))
                yield return new ValidationResult("Sorry. Not allowed to end a sentance with 'cow'.");
            if (Content.Contains(" calf."))
                yield return new ValidationResult("Sorry. Not allowed to end a sentance with 'calf'.");

        }
    }
}
