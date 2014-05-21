using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tests.DataClasses.Concrete;
using Tests.UiHelpers;

namespace Tests.DTOs
{
    public interface IDetailPostDto
    {
        [UIHint("HiddenInput")]
        [Key]
        int PostId { get; set; }

        [MinLength(2), MaxLength(128)]
        string Title { get; set; }

        [DataType(DataType.MultilineText)]
        string Content { get; set; }

        DateTime LastUpdated { get; }

        [UIHint("HiddenInput")]
        int BlogId { get; set; }

        ICollection<PostTagLink> AllocatedTags { get; set; }
        string BloggerName { get; }

        /// <summary>
        /// This allows a single blogger to be chosen from the list
        /// </summary>
        DropDownListType Bloggers { get; set; }

        MultiSelectListType UserChosenTags { get; set; }

        /// <summary>
        /// When it was last updated in DateTime format
        /// </summary>
        DateTime LastUpdatedUtc { get; }

        string TagNames { get; }
    }
}