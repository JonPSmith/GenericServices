using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GenericServices;
using GenericServices.Core;
using Tests.DataClasses.Concrete;

namespace Tests.DTOs.Concrete
{
    class TabNeededTagAndDelegatePostDto : EfGenericDto<Tag, TabNeededTagAndDelegatePostDto>
    {

        [Key]
        public int TagId { get; set; }

        [MaxLength(64)]
        [Required]
        [RegularExpression(@"\w*", ErrorMessage = "The slug must not contain spaces or non-alphanumeric characters.")]
        public string Slug { get; set; }

        [MaxLength(128)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// This requires DelegateDecompiler
        /// </summary>
        public IEnumerable<DelegateDecompilePostDto> Posts { get; set; }

        protected internal override CrudFunctions SupportedFunctions
        {
            get { return CrudFunctions.List ; }
        }

        protected override Type [] AssociatedDtoMappings
        {
            get { return new []{ typeof(SimpleTagDto), typeof(DelegateDecompilePostDto)}; }
        }

    }
}
