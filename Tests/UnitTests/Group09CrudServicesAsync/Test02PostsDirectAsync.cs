using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using GenericServices;
using GenericServices.Core;
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
                var status = await service.GetMany().Include(x => x.Blogger).TryManyWithPermissionCheckingAsync();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.Count().ShouldEqual(3);
                status.Result.First().Title.ShouldEqual(firstPost.Title);
                status.Result.First().Blogger.Name.ShouldEqual(firstPost.Blogger.Name);
                status.Result.First().Tags.ShouldEqual(null);
            }
        }

        [Test]
        public async Task Check03DetailDirectPostWhereOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailServiceAsync<Post>(db);
                var firstPost = db.Posts.First();

                //ATTEMPT
                var status = await service.GetDetailUsingWhereAsync(x => x.PostId == firstPost.PostId);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.PostId.ShouldEqual(firstPost.PostId);
                status.Result.Title.ShouldEqual(firstPost.Title);
            }
        }

        [Test]
        public async Task Check03DetailDirectPostKeyOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailServiceAsync<Post>(db);
                var firstPost = db.Posts.First();

                //ATTEMPT
                var status = await service.GetDetailAsync(firstPost.PostId);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.PostId.ShouldEqual(firstPost.PostId);
                status.Result.Title.ShouldEqual(firstPost.Title);
            }
        }

        [Test]
        public async Task Check05DetailDirectPostNotFoundBad()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailServiceAsync<Post>(db);

                //ATTEMPT
                var status = await service.GetDetailUsingWhereAsync(x => x.PostId == 0);

                //VERIFY
                status.IsValid.ShouldEqual(false, status.Errors);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("We could not find an entry using that filter. Has it been deleted by someone else?");
                status.Result.ShouldNotEqualNull();
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
