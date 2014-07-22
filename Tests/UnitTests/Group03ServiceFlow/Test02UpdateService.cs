using System;
using System.Linq;
using GenericServices.Core;
using GenericServices.Services;
using GenericServices.Services.Concrete;
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
                var filepath = TestFileHelpers.GetTestFileFilePath("DbContentSimple.xml");
                DataLayerInitialise.ResetDatabaseToTestData(db, filepath);
            }
        }

        [Test]
        public void Check01UpdateSetupFlowOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateSetupService<Tag, SimpleTagDto>(db);
                var firstTag = db.Tags.First();

                //ATTEMPT
                var dto = service.GetOriginal(firstTag.TagId);

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
                var service = new UpdateService<Tag, SimpleTagDto>(db);
                var dto = new SimpleTagDto();
                dto.SetSupportedFunctions(ServiceFunctions.None);

                //ATTEMPT
                var status = service.Update(dto);

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEndWith("is not supported in this mode.");
                dto.FunctionsCalledCommaDelimited.ShouldEqual("");
            }
        }

        [TestCase(InstrumentedOpFlags.NormalOperation, true, "FindItemTracked,CopyDtoToData")]
        [TestCase(InstrumentedOpFlags.FailOnCopyDtoToData, false, "FindItemTracked,CopyDtoToData,SetupSecondaryData")]
        public void Check02UpdateFlow(InstrumentedOpFlags errorFlag, bool isValid, string expectedFunctionsCalled)
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateService<Tag, SimpleTagDto>(db);
                var dto = new SimpleTagDto(errorFlag)
                {
                    TagId = db.Tags.First().TagId,
                    Name = "Test Name",
                    Slug = Guid.NewGuid().ToString("N")
                };

                //ATTEMPT
                //dto.CopyDtoToData(db, dto, new Tag());          //do this to preload AutoMapper
                var status = service.Update(dto);

                //VERIFY
                status.IsValid.ShouldEqual(isValid);
                dto.FunctionsCalledCommaDelimited.ShouldEqual(expectedFunctionsCalled);
            }
        }  

    }
}
