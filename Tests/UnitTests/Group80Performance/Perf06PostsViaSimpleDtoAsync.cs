using System;
using System.Data.Entity;
using System.Linq;
using GenericServices.ServicesAsync;
using GenericServices.ServicesAsync.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group80Performance
{
    class Perf06PostsViaSimpleDtoAsync
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
        public async void Perf01DetailPostOk()
        {
            int postId;
            using (var db = new SampleWebAppDb())
                postId = db.Posts.Include(x => x.Tags).AsNoTracking().First().PostId;

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailServiceAsync<Post, SimplePostDtoAsync>(db);

                //ATTEMPT
                var status = await service.GetDetailAsync(postId);
                status.Result.LogSpecificName("End");

                //VERIFY
                foreach (var log in status.Result.LogOfCalls) { Console.WriteLine(log); }
            }
        }

        [Test]
        public async void Perf05UpdateSetupOk()
        {
            int postId;
            using (var db = new SampleWebAppDb())
                postId = db.Posts.Include(x => x.Tags).AsNoTracking().First().PostId;

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateSetupServiceAsync<Post, SimplePostDtoAsync>(db);

                //ATTEMPT
                var status = await service.GetOriginalAsync(postId);
                status.Result.LogSpecificName("End");

                //VERIFY
                foreach (var log in status.Result.LogOfCalls) { Console.WriteLine(log); }
            }
        }

        [Test]
        public async void Perf06UpdateWithListDtoOk()
        {
            int postId;
            using (var db = new SampleWebAppDb())
                postId = db.Posts.Include(x => x.Tags).AsNoTracking().First().PostId;

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateServiceAsync<Post, SimplePostDtoAsync>(db);
                var setupService = new UpdateSetupServiceAsync<Post, SimplePostDtoAsync>(db);

                //ATTEMPT
                var setupStatus = await setupService.GetOriginalAsync(postId);
                setupStatus.Result.Title = Guid.NewGuid().ToString();
                var status = await service.UpdateAsync(setupStatus.Result);
                setupStatus.Result.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                foreach (var log in setupStatus.Result.LogOfCalls) { Console.WriteLine(log); }
                
            }
        }


    }
}
