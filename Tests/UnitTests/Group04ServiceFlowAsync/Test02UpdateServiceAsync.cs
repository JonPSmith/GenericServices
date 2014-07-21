using System;
using System.Linq;
using GenericServices.Core;
using GenericServices.Services;
using GenericServices.ServicesAsync;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group04ServiceFlowAsync
{
    class Test02UpdateServiceAsync
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
        public async void Check01UpdateSetupFlowOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateSetupServiceAsync<Tag, SimpleTagDtoAsync>(db);
                var firstTag = db.Tags.First();

                //ATTEMPT
                var dto = await service.GetOriginalAsync(firstTag.TagId);

                //VERIFY
                dto.FunctionsCalledCommaDelimited.ShouldEqual("CreateDtoAndCopyDataInAsync,SetupSecondaryDataAsync");
            }
        }

        [Test]
        public async void Check02UpdateFailOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateServiceAsync<Tag, SimpleTagDtoAsync>(db);
                var dto = new SimpleTagDtoAsync();
                dto.SetSupportedFunctions(ServiceFunctions.None);

                //ATTEMPT
                var status = await service.UpdateAsync(dto);

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEndWith("is not supported in this mode.");
                dto.FunctionsCalledCommaDelimited.ShouldEqual("");
            }
        }

        [TestCase(InstrumentedOpFlags.NormalOperation, true, "FindItemTrackedAsync,CopyDtoToDataAsync")]
        [TestCase(InstrumentedOpFlags.FailOnCopyDtoToData, false, "FindItemTrackedAsync,CopyDtoToDataAsync,SetupSecondaryDataAsync")]
        public async void Check02UpdateFlow(InstrumentedOpFlags errorFlag, bool isValid, string expectedFunctionsCalled)
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateServiceAsync<Tag, SimpleTagDtoAsync>(db);
                var dto = new SimpleTagDtoAsync(errorFlag)
                {
                    TagId = db.Tags.First().TagId,
                    Name = "Test Name",
                    Slug = Guid.NewGuid().ToString("N")
                };

                //ATTEMPT
                var status = await service.UpdateAsync(dto);

                //VERIFY
                status.IsValid.ShouldEqual(isValid);
                dto.FunctionsCalledCommaDelimited.ShouldEqual(expectedFunctionsCalled);
            }
        }  

    }
}
