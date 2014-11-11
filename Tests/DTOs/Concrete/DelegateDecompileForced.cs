using System.ComponentModel.DataAnnotations;
using GenericServices.Core;
using Tests.DataClasses.Concrete;

namespace Tests.DTOs.Concrete
{
    class DelegateDecompileForced : EfGenericDto<Tag, DelegateDecompileForced>
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

        protected internal override CrudFunctions SupportedFunctions
        {
            get { return CrudFunctions.List ; }
        }

        public override bool ForceNeedDecompile
        {
            get { return true; }
        }
    }
}
