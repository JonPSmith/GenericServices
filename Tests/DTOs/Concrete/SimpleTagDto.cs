using System.ComponentModel.DataAnnotations;
using GenericServices.Services;
using Tests.DataClasses.Concrete;

namespace Tests.DTOs.Concrete
{
    class SimpleTagDto : InstrumentedEfGenericDto<Tag, SimpleTagDto>
    {

        private ServiceFunctions _supportedFunctionsToUse = ServiceFunctions.AllCrud |
                                                    ServiceFunctions.DoAction |
                                                    ServiceFunctions.DoDbAction;

        public SimpleTagDto()
        {
        }

        public SimpleTagDto(InstrumentedOpFlags whereToFail) : base(whereToFail)
        {
        }


        [Key]
        public int TagId { get; set; }

        [MaxLength(64)]
        [Required]
        [RegularExpression(@"\w*", ErrorMessage = "The slug must not contain spaces or non-alphanumeric characters.")]
        public string Slug { get; set; }

        [MaxLength(128)]
        [Required]
        public string Name { get; set; }


        //--------------------------------------


        protected internal override ServiceFunctions SupportedFunctions
        {
            get { return _supportedFunctionsToUse; }
        }

        public void SetSupportedFunctions(ServiceFunctions allowedFunctions)
        {
            _supportedFunctionsToUse = allowedFunctions;
        }

    }
}
