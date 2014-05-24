using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GenericServices;
using GenericServices.Concrete;
using Tests.DataClasses.Concrete;

namespace Tests.DTOs.Concrete
{
    internal enum TestErrorsFlags { NoError, FailOnCopyDataToDto, FailOnCopyDtoToData }

    public class TestWithErrorsAndTrackingDto : EfGenericDto<Tag, TestWithErrorsAndTrackingDto>
    {
        private ServiceFunctions _supportedFunctionsToUse = ServiceFunctions.AllCrud | 
                                                            ServiceFunctions.RunTask |
                                                            ServiceFunctions.RunDbTask;

        private List<string> _logOfFunctionsCalled = new List<string>();

        //--------------------------------------
        //tag parts

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

        internal ServiceFunctions SupportedFunctionsToUse
        {
            get { return _supportedFunctionsToUse; }
            set { _supportedFunctionsToUse = value; }
        }

        internal string FunctionsCalledCommaDelimited { get { return string.Join(",", _logOfFunctionsCalled); } }

        internal TestErrorsFlags WhereToFail { get; set; }


        //------------
        //ctors

        public TestWithErrorsAndTrackingDto()
        {
        }

        internal TestWithErrorsAndTrackingDto(TestErrorsFlags whereToFail)
        {
            WhereToFail = whereToFail;
        }


        //---------------------------------------------------------------------
        //overridden methods

        protected internal override ServiceFunctions SupportedFunctions
        {
            get { return SupportedFunctionsToUse; }
        }

        protected internal override IQueryable<TestWithErrorsAndTrackingDto> BuildListQueryUntracked(IDbContextWithValidation context)
        {
            _logOfFunctionsCalled.Add("BuildListQueryUntracked");
            return base.BuildListQueryUntracked(context);
        }

        protected internal override ISuccessOrErrors CopyDataToDto(IDbContextWithValidation context, Tag source, TestWithErrorsAndTrackingDto destination)
        {
            _logOfFunctionsCalled.Add("CopyDataToDto");
            if (WhereToFail.HasFlag(TestErrorsFlags.FailOnCopyDataToDto))
                return new SuccessOrErrors().AddSingleError("Flag was set to fail here.");

            return base.CopyDataToDto(context, source, destination);
        }

        protected internal override ISuccessOrErrors CopyDtoToData(IDbContextWithValidation context, TestWithErrorsAndTrackingDto source, Tag destination)
        {
            _logOfFunctionsCalled.Add("CopyDtoToData");
            if (WhereToFail.HasFlag(TestErrorsFlags.FailOnCopyDtoToData))
                return new SuccessOrErrors().AddSingleError("Flag was set to fail here.");

            return base.CopyDtoToData(context, source, destination);
        }

        protected internal override TestWithErrorsAndTrackingDto CreateDtoAndCopyDataIn(IDbContextWithValidation context, Expression<Func<Tag, bool>> predicate)
        {
            _logOfFunctionsCalled.Add("CreateDtoAndCopyDataIn");
            var newDto = base.CreateDtoAndCopyDataIn(context, predicate);
            newDto._logOfFunctionsCalled = _logOfFunctionsCalled;
            newDto._supportedFunctionsToUse = _supportedFunctionsToUse;
            newDto.WhereToFail = WhereToFail;
            return newDto;
        }


        protected internal override Tag FindItemTracked(IDbContextWithValidation context)
        {
            _logOfFunctionsCalled.Add("FindItemTracked");
            return base.FindItemTracked(context);
        }

        protected internal override void SetupSecondaryData(IDbContextWithValidation db, TestWithErrorsAndTrackingDto dto)
        {
            _logOfFunctionsCalled.Add("SetupSecondaryData");
        }
    }
}
