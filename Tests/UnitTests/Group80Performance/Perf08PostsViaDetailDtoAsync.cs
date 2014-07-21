using System;
using System.Data.Entity;
using System.Linq;
using GenericServices.Services;
using GenericServices.ServicesAsync;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group80Performance
{
    class Perf08PostsViaDetailDtoAsync
    {

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            new DetailPostDtoAsync().CacheSetup();
        }

        [SetUp]
        public void SetUp()
        {
            using (var db = new SampleWebAppDb())
            {
                DataLayerInitialise.InitialiseThis();
                var filepath = TestFileHelpers.GetTestFileFilePath("DbContentSimple.xml");
                DataLayerInitialise.ResetDatabaseToTestData(db, filepath);
            }
        }


        [Test]
        public async void Perf03DetailPostOk()
        {
            int postId;
            using (var db = new SampleWebAppDb())
                postId = db.Posts.Include(x => x.Tags).AsNoTracking().First().PostId;

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailServiceAsync<Post, DetailPostDtoAsync>(db);

                //ATTEMPT
                var dto = await service.GetDetailAsync(postId);
                dto.LogSpecificName("End");

                //VERIFY
                foreach (var log in dto.LogOfCalls) { Console.WriteLine(log); }
            }
        }

        [Test]
        public async void Perf10CreateSetupServiceOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateSetupServiceAsync<Post, DetailPostDtoAsync>(db);

                //ATTEMPT
                var dto = await service.GetDtoAsync();
                dto.LogSpecificName("End");

                //VERIFY
                foreach (var log in dto.LogOfCalls) { Console.WriteLine(log); }
            }
        }


        [Test]
        public async void Perf11CreatePostOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP

                var service = new CreateServiceAsync<Post, DetailPostDtoAsync>(db);
                var setupService = new CreateSetupServiceAsync<Post, DetailPostDtoAsync>(db);

                //ATTEMPT
                var dto = await setupService.GetDtoAsync();
                dto.Title = Guid.NewGuid().ToString();
                dto.Content = "something to fill it as can't be empty";
                dto.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                dto.UserChosenTags.FinalSelection = db.Tags.Take(2).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = await service.CreateAsync(dto);
                dto.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                foreach (var log in dto.LogOfCalls) { Console.WriteLine(log); }
            }
        }



        [Test]
        public async void Perf16UpdateSetupServiceOk()
        {
            int postId;
            using (var db = new SampleWebAppDb())
                postId = db.Posts.Include(x => x.Tags).AsNoTracking().First().PostId;

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var setupService = new UpdateSetupServiceAsync<Post, DetailPostDtoAsync>(db);

                //ATTEMPT
                var dto = await setupService.GetOriginalAsync(postId);
                dto.LogSpecificName("End");

                //VERIFY
                foreach (var log in dto.LogOfCalls) { Console.WriteLine(log); }
            }
        }



        [Test]
        public async void Perf22UpdatePostAddTagOk()
        {
            int postId;
            using (var db = new SampleWebAppDb())
                postId = db.Posts.Include(x => x.Tags).AsNoTracking().First().PostId;

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var setupService = new UpdateSetupServiceAsync<Post, DetailPostDtoAsync>(db);
                var updateService = new UpdateServiceAsync<Post, DetailPostDtoAsync>(db);

                //ATTEMPT
                var dto = await setupService.GetOriginalAsync(postId);
                dto.Title = Guid.NewGuid().ToString();
                dto.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                dto.UserChosenTags.FinalSelection = db.Tags.Take(3).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = await updateService.UpdateAsync(dto);
                dto.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                foreach (var log in dto.LogOfCalls) { Console.WriteLine(log); }
            }
        }

    }
}
