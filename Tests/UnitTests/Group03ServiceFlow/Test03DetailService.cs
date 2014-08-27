using System;
using System.Linq;
using GenericServices.Services;
using GenericServices.Services.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group03ServiceFlow
{
    class Test03DetailService
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
        public void Check01DetailFlowOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailService<Tag, SimpleTagDto>(db);
                var firstTag = db.Tags.First();

                //ATTEMPT
                var status = service.GetDetailUsingWhere(x => x.TagId == firstTag.TagId);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.FunctionsCalledCommaDelimited.ShouldEqual("CreateDtoAndCopyDataIn");
            }
        }

        
    }
}
