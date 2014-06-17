using System;
using System.Data.Entity;
using System.Linq;
using GenericServices.Services;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group80Performance
{
    class Perf07PostsViaDetailDto
    {

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            new DetailPostDto().CacheSetup();
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
        public void Perf03DetailPostOk()
        {
            int postId;
            using (var db = new SampleWebAppDb())
                postId = db.Posts.Include(x => x.Tags).AsNoTracking().First().PostId;

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailService<Post, DetailPostDto>(db);

                //ATTEMPT
                var dto = service.GetDetail(x => x.PostId == postId);
                dto.LogSpecificName("End");

                //VERIFY
                foreach (var log in dto.LogOfCalls) { Console.WriteLine(log); }
            }
        }

        [Test]
        public void Perf10CreateSetupServiceOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateSetupService<Post, DetailPostDto>(db);

                //ATTEMPT
                var dto = service.GetDto();
                dto.LogSpecificName("End");

                //VERIFY
                foreach (var log in dto.LogOfCalls) { Console.WriteLine(log); }
            }
        }


        [Test]
        public void Perf11CreatePostOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP

                var service = new CreateService<Post, DetailPostDto>(db);
                var setupService = new CreateSetupService<Post, DetailPostDto>(db);

                //ATTEMPT
                var dto = setupService.GetDto();
                dto.Title = Guid.NewGuid().ToString();
                dto.Content = "something to fill it as can't be empty";
                dto.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                dto.UserChosenTags.FinalSelection = db.Tags.Take(2).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = service.Create(dto);
                dto.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                foreach (var log in dto.LogOfCalls) { Console.WriteLine(log); }
            }
        }



        [Test]
        public void Perf16UpdateSetupServiceOk()
        {
            int postId;
            using (var db = new SampleWebAppDb())
                postId = db.Posts.Include(x => x.Tags).AsNoTracking().First().PostId;

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var setupService = new UpdateSetupService<Post, DetailPostDto>(db);

                //ATTEMPT
                var dto = setupService.GetOriginal(x => x.PostId == postId);
                dto.LogSpecificName("End");

                //VERIFY
                foreach (var log in dto.LogOfCalls) { Console.WriteLine(log); }
            }
        }



        [Test]
        public void Perf22UpdatePostAddTagOk()
        {
            int postId;
            using (var db = new SampleWebAppDb())
                postId = db.Posts.Include(x => x.Tags).AsNoTracking().First().PostId;

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var setupService = new UpdateSetupService<Post, DetailPostDto>(db);
                var updateService = new UpdateService<Post, DetailPostDto>(db);

                //ATTEMPT
                var dto = setupService.GetOriginal(x => x.PostId == postId);
                dto.Title = Guid.NewGuid().ToString();
                dto.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                dto.UserChosenTags.FinalSelection = db.Tags.Take(3).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = updateService.Update(dto);
                dto.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                foreach (var log in dto.LogOfCalls) { Console.WriteLine(log); }
            }
        }

    }
}
