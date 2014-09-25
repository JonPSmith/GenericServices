using System;
using System.Data.Entity;
using System.Linq;
using GenericServices;
using GenericServices.Core;
using GenericServices.Services;
using GenericServices.Services.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group08CrudServices
{
    class Test02PostsDirect
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
        public void Check01DirectReferenceOk()
        {

            //SETUP    

            //ATTEMPT
            ICreateService<Post> createService = new CreateService<Post>(null);
            IDetailService<Post> detailService = new DetailService<Post>(null);
            IListService<Post> listService = new ListService<Post>(null);
            IUpdateService<Post> updateService = new UpdateService<Post>(null);

            //VERIFY
            (listService is IListService<Post>).ShouldEqual(true);
        }

        [Test]
        public void Check02ListDirectPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new ListService<Post>(db);
                var firstPost = db.Posts.Include(x => x.Blogger).First();

                //ATTEMPT
                var list = service.GetAll().Include(x => x.Blogger).ToList();
                
                //VERIFY
                list.Count().ShouldEqual(3);
                list.First().Title.ShouldEqual(firstPost.Title);
                list.First().Blogger.Name.ShouldEqual(firstPost.Blogger.Name);
                list.First().Tags.ShouldEqual(null);

            }
        }

        [Test]
        public void Check03DetailDirectPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailService<Post>(db);
                var firstPost = db.Posts.First();

                //ATTEMPT
                var status = service.GetDetail(firstPost.PostId);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.PostId.ShouldEqual(firstPost.PostId);
                status.Result.Title.ShouldEqual(firstPost.Title);
            }
        }

        [Test]
        public void Check03DetailDirectPostWhereOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailService<Post>(db);
                var firstPost = db.Posts.First();

                //ATTEMPT
                var status = service.GetDetailUsingWhere(x => x.PostId == firstPost.PostId);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.PostId.ShouldEqual(firstPost.PostId);
                status.Result.Title.ShouldEqual(firstPost.Title);
            }
        }

        [Test]
        public void Check05DetailDirectPostNotFoundBad()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailService<Post>(db);

                //ATTEMPT
                var status = service.GetDetailUsingWhere(x => x.PostId == 0);

                //VERIFY
                status.IsValid.ShouldEqual(false, status.Errors);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("We could not find an entry using that filter. Has it been deleted by someone else?");
                status.Result.ShouldNotEqualNull();
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
                var service = new UpdateService<Post>(db);

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
                var firstPostUntrackedWithIncludes = db.Posts.AsNoTracking().Include( x => x.Tags).First();
                var service = new UpdateService<Post>(db);

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
        public void Check08UpdateWithListDtoBad()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var firstPostUntracked = db.Posts.AsNoTracking().First();
                var service = new UpdateService<Post>(db);

                //ATTEMPT
                firstPostUntracked.Title = "Can't I ask a question?";
                var status = service.Update(firstPostUntracked);

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("Sorry, but you can't ask a question, i.e. the title can't end with '?'.");

            }
        }

        [Test]
        public void Check08CreateDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var service = new CreateService<Post>(db);
                var firstPostUntracked = db.Posts.Include( x => x.Tags).AsNoTracking().First();
                var tagsTracked = db.Tags.ToList().Where(x => firstPostUntracked.Tags.Any(y => y.TagId == x.TagId)).ToList();

                //ATTEMPT
                firstPostUntracked.Title = Guid.NewGuid().ToString();
                firstPostUntracked.Tags = tagsTracked;
                var status = service.Create(firstPostUntracked);

                //VERIFY
                status.IsValid.ShouldEqual(true);
                snap.CheckSnapShot(db,1,2);
                var updatedPost = db.Posts.OrderByDescending( x => x.PostId).Include(x => x.Tags).First();
                updatedPost.Title.ShouldEqual(firstPostUntracked.Title);
                updatedPost.BlogId.ShouldEqual(firstPostUntracked.BlogId);
                CollectionAssert.AreEqual(firstPostUntracked.Tags.Select(x => x.TagId), updatedPost.Tags.Select(x => x.TagId));
            }
        }

        [Test]
        public void Check10DeleteDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var firstPostUntracked = db.Posts.AsNoTracking().First();
                var service = new DeleteService(db);

                //ATTEMPT
                var status = service.Delete<Post>(firstPostUntracked.PostId);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.SuccessMessage.ShouldEqual("Successfully deleted Post.");
                snap.CheckSnapShot(db, -1,-2, 0, 0, -2);
            }
        }

    }
}
