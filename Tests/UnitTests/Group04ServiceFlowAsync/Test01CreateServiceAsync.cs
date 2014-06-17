using System;
using GenericServices.Services;
using GenericServices.ServicesAsync;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group04ServiceFlowAsync
{
    class Test01CreateServiceAsync
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
        public async void Check01CreateSetupFlowOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateSetupServiceAsync<Tag, SimpleTagDtoAsync>(db);

                //ATTEMPT
                var dto = await service.GetDtoAsync();

                //VERIFY
                dto.FunctionsCalledCommaDelimited.ShouldEqual("SetupSecondaryDataAsync");
            }
        }

        [Test]
        public async void Check02CreateFailOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateServiceAsync<Tag, SimpleTagDtoAsync>(db);
                var dto = new SimpleTagDtoAsync();
                dto.SetSupportedFunctions( ServiceFunctions.None);

                //ATTEMPT
                var status = await service.CreateAsync(dto);

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEndWith("is not supported in this mode.");
                dto.FunctionsCalledCommaDelimited.ShouldEqual("");
            }
        }

        [TestCase(InstrumentedOpFlags.NormalOperation, true, "CopyDtoToDataAsync")]
        [TestCase(InstrumentedOpFlags.FailOnCopyDtoToData, false, "CopyDtoToDataAsync,SetupSecondaryDataAsync")]  
        public async void Check02CreateFlow(InstrumentedOpFlags errorFlag, bool isValid, string expectedFunctionsCalled)
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateServiceAsync<Tag, SimpleTagDtoAsync>(db);
                var dto = new SimpleTagDtoAsync(errorFlag)
                {
                    Name = "Test Name",
                    Slug = Guid.NewGuid().ToString("N")
                };

                //ATTEMPT
                var status = await service.CreateAsync(dto);

                //VERIFY
                status.IsValid.ShouldEqual(isValid);
                dto.FunctionsCalledCommaDelimited.ShouldEqual(expectedFunctionsCalled);
            }
        }  

    }
}
