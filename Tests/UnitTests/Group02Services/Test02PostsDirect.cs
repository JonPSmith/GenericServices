using System;
using System.Data.Entity;
using System.Linq;
using GenericServices;
using GenericServices.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group02Services
{
    class Test02PostsDirect
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

                //ATTEMPT
                var query = service.GetList().Include(x => x.Blogger);
                var list = query.ToList();

                //VERIFY
                list.Count.ShouldEqual(3);
                list[0].Title.ShouldEqual("First great post");
                list[0].Blogger.Name.ShouldEqual("Jon Smith");
                list[0].AllocatedTags.ShouldEqual(null);

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
                var firstPostUntrackedWithIncludes = db.Posts.AsNoTracking().Include( x => x.AllocatedTags).First();
                var service = new UpdateService<Post>(db);

                //ATTEMPT
                firstPostUntrackedNoIncludes.Title = Guid.NewGuid().ToString();
                var status = service.Update(firstPostUntrackedNoIncludes);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db);
                var updatedPost = db.Posts.Include(x => x.AllocatedTags).First();
                updatedPost.Title.ShouldEqual(firstPostUntrackedNoIncludes.Title);
                updatedPost.Content.ShouldEqual(firstPostUntrackedWithIncludes.Content);
                updatedPost.Blogger.ShouldNotEqualNull();
                updatedPost.Blogger.Name.ShouldEqual(firstPostUntrackedWithIncludes.Blogger.Name);
                CollectionAssert.AreEqual(firstPostUntrackedWithIncludes.AllocatedTags.Select(x => x.TagId), updatedPost.AllocatedTags.Select(x => x.TagId));

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
                var firstPostUntracked = db.Posts.Include( x => x.AllocatedTags).AsNoTracking().First();
                var tagsTracked = db.Tags.ToList().Where(x => firstPostUntracked.AllocatedTags.Any(y => y.TagId == x.TagId)).ToList();

                //ATTEMPT
                firstPostUntracked.Title = Guid.NewGuid().ToString();
                firstPostUntracked.AllocatedTags =
                    tagsTracked.Select(x => new PostTagLink {InPost = firstPostUntracked, HasTag = x}).ToList();
                var status = service.Create(firstPostUntracked);

                //VERIFY
                status.IsValid.ShouldEqual(true);
                snap.CheckSnapShot(db,1,2);
                var updatedPost = db.Posts.OrderByDescending( x => x.PostId).Include(x => x.AllocatedTags).First();
                updatedPost.Title.ShouldEqual(firstPostUntracked.Title);
                updatedPost.BlogId.ShouldEqual(firstPostUntracked.BlogId);
                CollectionAssert.AreEqual(firstPostUntracked.AllocatedTags.Select(x => x.TagId), updatedPost.AllocatedTags.Select(x => x.TagId));
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
                var service = new DeleteService<Post>(db);

                //ATTEMPT
                var status = service.Delete(firstPostUntracked.PostId);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.SuccessMessage.ShouldEqual("Successfully deleted Post.");
                snap.CheckSnapShot(db, -1,-2);
            }
        }

    }
}
