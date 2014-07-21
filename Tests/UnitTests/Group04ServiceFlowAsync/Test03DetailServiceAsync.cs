using System.Linq;
using GenericServices.Services;
using GenericServices.ServicesAsync;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group04ServiceFlowAsync
{
    class Test03DetailServiceAsync
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
        public async void Check01DetailFlowOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailServiceAsync<Tag, SimpleTagDtoAsync>(db);
                var firstTag = db.Tags.First();

                //ATTEMPT
                var dto = await service.GetDetailAsync(firstTag.TagId);

                //VERIFY
                dto.FunctionsCalledCommaDelimited.ShouldEqual("CreateDtoAndCopyDataInAsync");
            }
        }

        
    }
}
