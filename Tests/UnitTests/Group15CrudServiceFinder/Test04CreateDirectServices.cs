using System;
using System.Data.Entity;
using System.Linq;
using GenericServices.Services;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group15CrudServiceFinder
{
    class Test04CreateDirectServices
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
        public void Check02ListDirectPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new ListService(db);
                var firstPost = db.Posts.Include(x => x.Blogger).First();

                //ATTEMPT
                var query = service.GetList<Post>().Include(x => x.Blogger);
                var list = query.ToList();

                //VERIFY
                list.Count.ShouldEqual(3);
                list[0].Title.ShouldEqual(firstPost.Title);
                list[0].Blogger.Name.ShouldEqual(firstPost.Blogger.Name);
                list[0].Tags.ShouldEqual(null);

            }
        }

        [Test]
        public void Check03DetailDirectPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailService(db);
                var firstPost = db.Posts.First();

                //ATTEMPT
                var item = service.GetDetail<Post>(firstPost.PostId);

                //VERIFY
                item.PostId.ShouldEqual(firstPost.PostId);
                item.Title.ShouldEqual(firstPost.Title);
            }
        }

        [Test]
        public void Check06UpdateDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var firstPostUntracked = db.Posts.AsNoTracking().First();
                var service = new UpdateService(db);

                //ATTEMPT
                firstPostUntracked.Title = Guid.NewGuid().ToString();
                var status = service.Update(firstPostUntracked);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.SuccessMessage.ShouldEqual("Successfully updated Post.");
                snap.CheckSnapShot(db);

            }
        }

        [Test]
        public void Check07UpdateDirectPostCorrectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var firstPostUntrackedNoIncludes = db.Posts.AsNoTracking().First();
                var firstPostUntrackedWithIncludes = db.Posts.AsNoTracking().Include(x => x.Tags).First();
                var service = new UpdateService(db);

                //ATTEMPT
                firstPostUntrackedNoIncludes.Title = Guid.NewGuid().ToString();
                var status = service.Update(firstPostUntrackedNoIncludes);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db);
                var updatedPost = db.Posts.Include(x => x.Tags).First();
                updatedPost.Title.ShouldEqual(firstPostUntrackedNoIncludes.Title);
                updatedPost.Content.ShouldEqual(firstPostUntrackedWithIncludes.Content);
                updatedPost.Blogger.ShouldNotEqualNull();
                updatedPost.Blogger.Name.ShouldEqual(firstPostUntrackedWithIncludes.Blogger.Name);
                CollectionAssert.AreEqual(firstPostUntrackedWithIncludes.Tags.Select(x => x.TagId), updatedPost.Tags.Select(x => x.TagId));
            }
        }

        [Test]
        public void Check08UpdateSetupServiceDto()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var firstPostUntracked = db.Posts.AsNoTracking().First();
                var service = new UpdateSetupService(db);

                //ATTEMPT
                firstPostUntracked.Title = "Can't I ask a question?";
                var result = service.GetOriginal<Post>(firstPostUntracked.PostId);

                //VERIFY
                result.ShouldNotEqualNull();
            }
        }

        [Test]
        public void Check10CreateSetupDtoOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateSetupService(db);

                //ATTEMPT
                var result = service.GetDto<SimplePostDto>();

                //VERIFY
                result.ShouldNotEqualNull();
            }
        }

        [Test]
        public void Check11CreateDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var service = new CreateService(db);
                var firstPostUntracked = db.Posts.Include(x => x.Tags).AsNoTracking().First();
                var tagsTracked = db.Tags.ToList().Where(x => firstPostUntracked.Tags.Any(y => y.TagId == x.TagId)).ToList();

                //ATTEMPT
                firstPostUntracked.Title = Guid.NewGuid().ToString();
                firstPostUntracked.Tags = tagsTracked;
                var status = service.Create(firstPostUntracked);

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
