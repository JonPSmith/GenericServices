using System;
using System.Data.Entity;
using System.Linq;
using GenericServices;
using GenericServices.Services;
using GenericServices.Services.Concrete;
using GenericServices.ServicesAsync;
using GenericServices.ServicesAsync.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group09CrudServicesAsync
{
    class Test02PostsDirectAsync
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
            ICreateServiceAsync<Post> createService = new CreateServiceAsync<Post>(null);
            IDetailServiceAsync<Post> detailService = new DetailServiceAsync<Post>(null);
            IDeleteServiceAsync deleteService = new DeleteServiceAsync(null);
            IUpdateServiceAsync<Post> updateService = new UpdateServiceAsync<Post>(null);

            //VERIFY
            (updateService is IUpdateServiceAsync<Post>).ShouldEqual(true);
        }

        [Test]
        public async void Check02ListDirectPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new ListService<Post>(db);
                var firstPost = db.Posts.Include(x => x.Blogger).First();

                //ATTEMPT
                var query = service.GetList().Include(x => x.Blogger);
                var list = await query.ToListAsync();

                //VERIFY
                list.Count.ShouldEqual(3);
                list[0].Title.ShouldEqual(firstPost.Title);
                list[0].Blogger.Name.ShouldEqual(firstPost.Blogger.Name);
                list[0].Tags.ShouldEqual(null);

            }
        }

        [Test]
        public async void Check06UpdateDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var firstPostUntracked = db.Posts.AsNoTracking().First();
                var service = new UpdateServiceAsync<Post>(db);

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
        public async void Check07UpdateDirectPostCorrectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var firstPostUntrackedNoIncludes = db.Posts.AsNoTracking().First();
                var firstPostUntrackedWithIncludes = db.Posts.AsNoTracking().Include( x => x.Tags).First();
                var service = new UpdateServiceAsync<Post>(db);

                //ATTEMPT
                firstPostUntrackedNoIncludes.Title = Guid.NewGuid().ToString();
                var status = await service.UpdateAsync(firstPostUntrackedNoIncludes);

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
        public async void Check08CreateDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var service = new CreateServiceAsync<Post>(db);
                var firstPostUntracked = db.Posts.Include( x => x.Tags).AsNoTracking().First();
                var tagsTracked = db.Tags.ToList().Where(x => firstPostUntracked.Tags.Any(y => y.TagId == x.TagId)).ToList();

                //ATTEMPT
                firstPostUntracked.Title = Guid.NewGuid().ToString();
                firstPostUntracked.Tags = tagsTracked;
                var status = await service.CreateAsync(firstPostUntracked);

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
        public async void Check10DeleteDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var firstPostUntracked = db.Posts.AsNoTracking().First();
                var service = new DeleteServiceAsync(db);

                //ATTEMPT
                var status = await service.DeleteAsync<Post>(firstPostUntracked.PostId);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.SuccessMessage.ShouldEqual("Successfully deleted Post.");
                snap.CheckSnapShot(db, -1,-2, 0, 0, -2);
            }
        }

    }
}
