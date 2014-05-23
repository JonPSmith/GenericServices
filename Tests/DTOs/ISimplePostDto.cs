using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using GenericServices;
using Tests.DataClasses.Concrete;

namespace Tests.DTOs
{
    public interface ISimplePostDto
    {
        [UIHint("HiddenInput")]
        [Key]
        int PostId { get; set; }

        string BloggerName { get; }

        [MinLength(2), MaxLength(128)]
        string Title { get; set; }

        List<PostTagLink> AllocatedTags { get; }
        DateTime LastUpdated { get; }

        /// <summary>
        /// When it was last updated in DateTime format
        /// </summary>
        DateTime LastUpdatedUtc { get; }

        string TagNames { get; }
    }
}