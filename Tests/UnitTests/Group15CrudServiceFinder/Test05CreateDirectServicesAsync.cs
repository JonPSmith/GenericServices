using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using GenericServices.Services;
using GenericServices.ServicesAsync;
using GenericServices.ServicesAsync.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group15CrudServiceFinder
{
    class Test05CreateDirectServicesAsync
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
        public async Task Check03DetailDirectPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailServiceAsync(db);
                var firstPost = db.Posts.First();

                //ATTEMPT
                var item = await service.GetDetailAsync<Post>(firstPost.PostId);

                //VERIFY
                item.PostId.ShouldEqual(firstPost.PostId);
                item.Title.ShouldEqual(firstPost.Title);
            }
        }

        [Test]
        public async Task Check04DetailDirectDtoOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailServiceAsync(db);
                var firstPost = db.Posts.Include(x => x.Tags).AsNoTracking().First();

                //ATTEMPT
                var dto = await service.GetDetailAsync<SimplePostDtoAsync>(firstPost.PostId);

                //VERIFY
                dto.PostId.ShouldEqual(firstPost.PostId);
                dto.BloggerName.ShouldEqual(firstPost.Blogger.Name);
                dto.Title.ShouldEqual(firstPost.Title);
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.TagId), dto.Tags.Select(x => x.TagId));
            }
        }

        //-------------
        //update

        [Test]
        public async Task Check05UpdateSetupServicePost()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var firstPostUntracked = db.Posts.AsNoTracking().First();
                var service = new UpdateSetupServiceAsync(db);

                //ATTEMPT
                var result = await service.GetOriginalAsync<Post>(firstPostUntracked.PostId);

                //VERIFY
                result.ShouldNotEqualNull();
                result.PostId.ShouldEqual(firstPostUntracked.PostId);
            }
        }

        [Test]
        public async Task Check05UpdateSetupServiceDto()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var firstPostUntracked = db.Posts.AsNoTracking().First();
                var service = new UpdateSetupServiceAsync(db);

                //ATTEMPT
                var result = await service.GetOriginalAsync<SimplePostDtoAsync>(firstPostUntracked.PostId);

                //VERIFY
                result.ShouldNotEqualNull();
                result.PostId.ShouldEqual(firstPostUntracked.PostId);
            }
        }

        [Test]
        public async Task Check06UpdateDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var firstPostUntracked = db.Posts.AsNoTracking().First();
                var service = new UpdateServiceAsync(db);

                //ATTEMPT
                firstPostUntracked.Title = Guid.NewGuid().ToString();
                var status = await service.UpdateAsync(firstPostUntracked);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.SuccessMessage.ShouldEqual("Successfully updated Post.");
                snap.CheckSnapShot(db);

            }
        }

        [Test]
        public async Task Check07UpdateDirectDtoOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var firstPostUntrackedNoIncludes = db.Posts.AsNoTracking().First();
                var service = new UpdateServiceAsync(db);
                var dto = await (new UpdateSetupServiceAsync(db)).GetOriginalAsync<SimplePostDtoAsync>(firstPostUntrackedNoIncludes.PostId);

                //ATTEMPT
                dto.Title = Guid.NewGuid().ToString();
                var status = await service.UpdateAsync(dto);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db);
                var updatedPost = db.Posts.Include(x => x.Tags).First();
                updatedPost.Title.ShouldEqual(dto.Title);
                updatedPost.Content.ShouldEqual(firstPostUntrackedNoIncludes.Content);
            }
        }

        //--------------------------------------------
        //create

        [Test]
        public async Task Check10CreateSetupDtoOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateSetupServiceAsync(db);

                //ATTEMPT
                var result = await service.GetDtoAsync<SimplePostDtoAsync>();

                //VERIFY
                result.ShouldNotEqualNull();
            }
        }

        [Test]
        public async Task Check11CreateDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var service = new CreateServiceAsync(db);
                var firstPostUntracked = db.Posts.Include(x => x.Tags).AsNoTracking().First();
                var tagsTracked = db.Tags.ToList().Where(x => firstPostUntracked.Tags.Any(y => y.TagId == x.TagId)).ToList();

                //ATTEMPT
                firstPostUntracked.Title = Guid.NewGuid().ToString();
                firstPostUntracked.Tags = tagsTracked;
                var status = await service.CreateAsync(firstPostUntracked);

                //VERIFY
                status.IsValid.ShouldEqual(true);
                snap.CheckSnapShot(db, 1, 2);
                var updatedPost = db.Posts.OrderByDescending(x => x.PostId).Include(x => x.Tags).First();
                updatedPost.Title.ShouldEqual(firstPostUntracked.Title);
                updatedPost.BlogId.ShouldEqual(firstPostUntracked.BlogId);
                CollectionAssert.AreEqual(firstPostUntracked.Tags.Select(x => x.TagId), updatedPost.Tags.Select(x => x.TagId));
            }
        }

    }
}
