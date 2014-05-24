using System;
using System.Linq;
using GenericServices.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group03ServiceFlow
{
    class Test02UpdateService
    {

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            using (var db = new SampleWebAppDb())
            {
                DataLayerInitialise.InitialiseThis();
                DataLayerInitialise.ResetDatabaseToTestData(db);
            }
        }

        [Test]
        public void Check01UpdateSetupFlowOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateSetupService<Tag, TestWithErrorsAndTrackingDto>(db);
                var firstTag = db.Tags.First();

                //ATTEMPT
                var dto = service.GetOriginal(x => x.TagId == firstTag.TagId);

                //VERIFY
                dto.FunctionsCalledCommaDelimited.ShouldEqual("CreateDtoAndCopyDataIn,SetupSecondaryData");
            }
        }

        [Test]
        public void Check02UpdateFailOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateService<Tag, TestWithErrorsAndTrackingDto>(db);
                var dto = new TestWithErrorsAndTrackingDto();
                dto.SupportedFunctionsToUse = ServiceFunctions.None;

                //ATTEMPT
                var status = service.Update(dto);

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEndWith("is not supported in this mode.");
                dto.FunctionsCalledCommaDelimited.ShouldEqual("");
            }
        }

        [TestCase(TestErrorsFlags.NoError, true, "FindItemTracked,CopyDtoToData")]
        [TestCase(TestErrorsFlags.FailOnCopyDtoToData, false, "FindItemTracked,CopyDtoToData,SetupSecondaryData")]
        public void Check02UpdateFlow(TestErrorsFlags errorFlag, bool isValid, string expectedFunctionsCalled)
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateService<Tag, TestWithErrorsAndTrackingDto>(db);
                var dto = new TestWithErrorsAndTrackingDto(errorFlag)
                {
                    TagId = db.Tags.First().TagId,
                    Name = "Test Name",
                    Slug = Guid.NewGuid().ToString("N")
                };

                //ATTEMPT
                var status = service.Update(dto);

                //VERIFY
                status.IsValid.ShouldEqual(isValid);
                dto.FunctionsCalledCommaDelimited.ShouldEqual(expectedFunctionsCalled);
            }
        }  

    }
}
