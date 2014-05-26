using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using GenericServices;
using GenericServices.Concrete;
using Tests.DataClasses.Concrete;

[assembly: InternalsVisibleTo("Tests")]

namespace Tests.DTOs.Concrete
{
    public class SimplePostDto : InstrumentedEfGenericDto<Post, SimplePostDto>, ISimplePostDto
    {

        [UIHint("HiddenInput")]
        [Key]
        public int PostId { get; set; }

        public string BloggerName { get; internal set; }

        [MinLength(2), MaxLength(128)]
        public string Title { get; set; }                   //only the Title can be updated

        public ICollection<Tag> Tags { get; internal set; }

        public DateTime LastUpdated { get; internal set; }

        /// <summary>
        /// When it was last updated in DateTime format
        /// </summary>
        public DateTime LastUpdatedUtc { get { return DateTime.SpecifyKind(LastUpdated, DateTimeKind.Utc); } }

        public string TagNames { get { return string.Join(", ", Tags.Select(x => x.Name)); } }

        //----------------------------------------------
        //overridden properties or methods

        internal protected override ServiceFunctions SupportedFunctions
        {
            get { return ServiceFunctions.AllCrudButCreate | ServiceFunctions.DoesNotNeedSetup; }
        }
    }
}
