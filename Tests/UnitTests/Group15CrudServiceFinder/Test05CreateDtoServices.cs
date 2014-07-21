using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using GenericServices.Services;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group15CrudServiceFinder
{
    class Test05CreateDtoServices
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
        public void Check01ListDtoPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new ListService(db);

                //ATTEMPT
                var query = service.GetList<SimplePostDto>();
                var list = query.ToList();

                //VERIFY
                list.Count.ShouldEqual(3);
                list[0].Title.ShouldEqual("First great post");
                list[0].BloggerName.ShouldEqual("Jon Smith");
                list[0].TagNames.ShouldEqual("Ugly post, Good post");
                list[0].LastUpdatedUtc.Kind.ShouldEqual(DateTimeKind.Utc);

            }
        }

        [Test]
        public void Check02DetailPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailService(db);
                var firstPost = db.Posts.Include(x => x.Tags).AsNoTracking().First();

                //ATTEMPT
                var dto = service.GetDetail<SimplePostDto>(firstPost.PostId);

                //VERIFY
                dto.PostId.ShouldEqual(firstPost.PostId);
                dto.BloggerName.ShouldEqual(firstPost.Blogger.Name);
                dto.Title.ShouldEqual(firstPost.Title);
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.TagId), dto.Tags.Select(x => x.TagId));
            }
        }

        //[Test]
        //public void Check06UpdateDirectOk()
        //{
        //    using (var db = new SampleWebAppDb())
        //    {
        //        //SETUP
        //        var snap = new DbSnapShot(db);
        //        var firstPostUntracked = db.Posts.AsNoTracking().First();
        //        var service = new UpdateService<Post>(db);

        //        //ATTEMPT
        //        firstPostUntracked.Title = Guid.NewGuid().ToString();
        //        var status = service.Update(firstPostUntracked);

        //        //VERIFY
        //        status.IsValid.ShouldEqual(true, status.Errors);
        //        status.SuccessMessage.ShouldEqual("Successfully updated Post.");
        //        snap.CheckSnapShot(db);

        //    }
        //}

        //[Test]
        //public void Check07UpdateDirectPostCorrectOk()
        //{
        //    using (var db = new SampleWebAppDb())
        //    {
        //        //SETUP
        //        var snap = new DbSnapShot(db);
        //        var firstPostUntrackedNoIncludes = db.Posts.AsNoTracking().First();
        //        var firstPostUntrackedWithIncludes = db.Posts.AsNoTracking().Include(x => x.Tags).First();
        //        var service = new UpdateService<Post>(db);

        //        //ATTEMPT
        //        firstPostUntrackedNoIncludes.Title = Guid.NewGuid().ToString();
        //        var status = service.Update(firstPostUntrackedNoIncludes);

        //        //VERIFY
        //        status.IsValid.ShouldEqual(true, status.Errors);
        //        snap.CheckSnapShot(db);
        //        var updatedPost = db.Posts.Include(x => x.Tags).First();
        //        updatedPost.Title.ShouldEqual(firstPostUntrackedNoIncludes.Title);
        //        updatedPost.Content.ShouldEqual(firstPostUntrackedWithIncludes.Content);
        //        updatedPost.Blogger.ShouldNotEqualNull();
        //        updatedPost.Blogger.Name.ShouldEqual(firstPostUntrackedWithIncludes.Blogger.Name);
        //        CollectionAssert.AreEqual(firstPostUntrackedWithIncludes.Tags.Select(x => x.TagId), updatedPost.Tags.Select(x => x.TagId));

        //    }
        //}

        //[Test]
        //public void Check08UpdateWithListDtoBad()
        //{
        //    using (var db = new SampleWebAppDb())
        //    {
        //        //SETUP
        //        var firstPostUntracked = db.Posts.AsNoTracking().First();
        //        var service = new UpdateService<Post>(db);

        //        //ATTEMPT
        //        firstPostUntracked.Title = "Can't I ask a question?";
        //        var status = service.Update(firstPostUntracked);

        //        //VERIFY
        //        status.IsValid.ShouldEqual(false);
        //        status.Errors.Count.ShouldEqual(1);
        //        status.Errors[0].ErrorMessage.ShouldEqual("Sorry, but you can't ask a question, i.e. the title can't end with '?'.");

        //    }
        //}

        //[Test]
        //public void Check08CreateDirectOk()
        //{
        //    using (var db = new SampleWebAppDb())
        //    {
        //        //SETUP
        //        var snap = new DbSnapShot(db);
        //        var service = new CreateService<Post>(db);
        //        var firstPostUntracked = db.Posts.Include(x => x.Tags).AsNoTracking().First();
        //        var tagsTracked = db.Tags.ToList().Where(x => firstPostUntracked.Tags.Any(y => y.TagId == x.TagId)).ToList();

        //        //ATTEMPT
        //        firstPostUntracked.Title = Guid.NewGuid().ToString();
        //        firstPostUntracked.Tags = tagsTracked;
        //        var status = service.Create(firstPostUntracked);

        //        //VERIFY
        //        status.IsValid.ShouldEqual(true);
        //        snap.CheckSnapShot(db, 1, 2);
        //        var updatedPost = db.Posts.OrderByDescending(x => x.PostId).Include(x => x.Tags).First();
        //        updatedPost.Title.ShouldEqual(firstPostUntracked.Title);
        //        updatedPost.BlogId.ShouldEqual(firstPostUntracked.BlogId);
        //        CollectionAssert.AreEqual(firstPostUntracked.Tags.Select(x => x.TagId), updatedPost.Tags.Select(x => x.TagId));
        //    }
        //}

        //[Test]
        //public void Check10DeleteDirectOk()
        //{
        //    using (var db = new SampleWebAppDb())
        //    {
        //        //SETUP
        //        var snap = new DbSnapShot(db);
        //        var firstPostUntracked = db.Posts.AsNoTracking().First();
        //        var service = new DeleteService<Post>(db);

        //        //ATTEMPT
        //        var status = service.Delete(firstPostUntracked.PostId);

        //        //VERIFY
        //        status.IsValid.ShouldEqual(true, status.Errors);
        //        status.SuccessMessage.ShouldEqual("Successfully deleted Post.");
        //        snap.CheckSnapShot(db, -1, -2, 0, 0, -2);
        //    }
        //}

    }
}
