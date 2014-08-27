using System;
using System.Data.Entity;
using System.Linq;
using GenericServices.Services;
using GenericServices.Services.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group80Performance
{
    class Perf05PostsViaSimpleDto
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
            new SimplePostDto().CacheSetup();
        }
        
        //--------------------------------------------------------


        [Test]
        public void Perf01DetailPostOk()
        {
            int postId;
            using (var db = new SampleWebAppDb())
                postId = db.Posts.Include(x => x.Tags).AsNoTracking().First().PostId;

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailService<Post, SimplePostDto>(db);

                //ATTEMPT
                var status = service.GetDetailUsingWhere(x => x.PostId == postId);
                status.Result.LogSpecificName("End");

                //VERIFY
                foreach (var log in status.Result.LogOfCalls) { Console.WriteLine(log); }
            }
        }

        [Test]
        public void Perf05UpdateSetupOk()
        {
            int postId;
            using (var db = new SampleWebAppDb())
                postId = db.Posts.Include(x => x.Tags).AsNoTracking().First().PostId;

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateSetupService<Post, SimplePostDto>(db);

                //ATTEMPT
                var status = service.GetOriginal(postId);
                status.Result.LogSpecificName("End");

                //VERIFY
                foreach (var log in status.Result.LogOfCalls) { Console.WriteLine(log); }
            }
        }

        [Test]
        public void Perf06UpdateWithListDtoOk()
        {
            int postId;
            using (var db = new SampleWebAppDb())
                postId = db.Posts.Include(x => x.Tags).AsNoTracking().First().PostId;

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var service = new UpdateService<Post, SimplePostDto>(db);
                var setupService = new UpdateSetupService<Post, SimplePostDto>(db);

                //ATTEMPT
                var setupStatus = setupService.GetOriginal(postId);
                setupStatus.Result.Title = Guid.NewGuid().ToString();
                var status = service.Update(setupStatus.Result);
                setupStatus.Result.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                foreach (var log in setupStatus.Result.LogOfCalls) { Console.WriteLine(log); }
                
            }
        }


    }
}
