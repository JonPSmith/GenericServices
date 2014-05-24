using System;
using GenericServices.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;
using Tests.TestOnlyDTOs;

namespace Tests.UnitTests.Group03ServiceFlow
{
    class Test01CreateService
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
        public void Check01CreateSetupFlowOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateSetupService<Tag, TestWithErrorsAndTrackingDto>(db);

                //ATTEMPT
                var dto = service.GetDto();

                //VERIFY
                dto.FunctionsCalledCommaDelimited.ShouldEqual("SetupSecondaryData");
            }
        }

        [Test]
        public void Check02CreateFailOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateService<Tag, TestWithErrorsAndTrackingDto>(db);
                var dto = new TestWithErrorsAndTrackingDto();
                dto.SupportedFunctionsToUse = ServiceFunctions.None;

                //ATTEMPT
                var status = service.Create(dto);

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEndWith("is not supported in this mode.");
                dto.FunctionsCalledCommaDelimited.ShouldEqual("");
            }
        }

        [TestCase(TestErrorsFlags.NoError, true, "CopyDtoToData")]
        [TestCase(TestErrorsFlags.FailOnCopyDtoToData, false, "CopyDtoToData,SetupSecondaryData")]  
        public void Check02CreateFlow(TestErrorsFlags errorFlag, bool isValid, string expectedFunctionsCalled)
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateService<Tag, TestWithErrorsAndTrackingDto>(db);
                var dto = new TestWithErrorsAndTrackingDto(errorFlag)
                {
                    Name = "Test Name",
                    Slug = Guid.NewGuid().ToString("N")
                };

                //ATTEMPT
                var status = service.Create(dto);

                //VERIFY
                status.IsValid.ShouldEqual(isValid);
                dto.FunctionsCalledCommaDelimited.ShouldEqual(expectedFunctionsCalled);
            }
        }  

    }
}
