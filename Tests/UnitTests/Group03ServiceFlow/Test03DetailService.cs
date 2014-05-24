using System;
using System.Linq;
using GenericServices.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;
using Tests.TestOnlyDTOs;

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
                DataLayerInitialise.ResetDatabaseToTestData(db);
            }
        }

        [Test]
        public void Check01DetailFlowOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailService<Tag, TestWithErrorsAndTrackingDto>(db);
                var firstTag = db.Tags.First();

                //ATTEMPT
                var dto = service.GetDetail(x => x.TagId == firstTag.TagId);

                //VERIFY
                dto.FunctionsCalledCommaDelimited.ShouldEqual("CreateDtoAndCopyDataIn");
            }
        }

        
    }
}
