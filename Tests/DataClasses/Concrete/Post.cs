#region licence
// The MIT License (MIT)
// 
// Filename: Post.cs
// Date Created: 2014/05/19
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DelegateDecompiler;
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

        public ICollection<Tag> Tags { get; set; }

        public override string ToString()
        {
            return string.Format("PostId: {0}, Title: {1}, BlogId: {2}, Blogger: {3}, AllocatedTags: {4}", 
                PostId, Title, BlogId, Blogger == null ? "null" : Blogger.Name, Tags == null ? "null" : Tags.Count().ToString());
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //Note that Tags may be null if the read din't include them, in which case we can't check them
            if (Tags != null && !Tags.Any())
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
