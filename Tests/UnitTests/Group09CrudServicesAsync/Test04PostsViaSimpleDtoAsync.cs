using System;
using System.Data.Entity;
using System.Linq;
using GenericServices;
using GenericServices.ServicesAsync;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group09CrudServicesAsync
{
    class Test04PostsViaSimpleDtoAsync
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
            new SimplePostDtoAsync().CacheSetup();
        }

        [Test]
        public void Check01ReferenceOk()
        {

            //SETUP    

            //ATTEMPT
            ICreateServiceAsync<Post, SimplePostDtoAsync> createService = new CreateServiceAsync<Post, SimplePostDtoAsync>(null);
            IDetailServiceAsync<Post, SimplePostDtoAsync> detailService = new DetailServiceAsync<Post, SimplePostDtoAsync>(null);
            IUpdateServiceAsync<Post, SimplePostDtoAsync> updateService = new UpdateServiceAsync<Post, SimplePostDtoAsync>(null);

            //VERIFY
            (createService is ICreateServiceAsync<Post, SimplePostDtoAsync>).ShouldEqual(true);
        }
        
        //--------------------------------------------------------

        [Test]
        public async void Check02DetailPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailServiceAsync<Post, SimplePostDtoAsync>(db);
                var firstPost = db.Posts.Include(x => x.Tags).AsNoTracking().First();

                //ATTEMPT
                var dto = await service.GetDetailAsync(firstPost.PostId);
                dto.LogSpecificName("End");

                //VERIFY
                dto.PostId.ShouldEqual(firstPost.PostId);
                dto.BloggerName.ShouldEqual(firstPost.Blogger.Name);
                dto.Title.ShouldEqual(firstPost.Title);
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.TagId), dto.Tags.Select(x => x.TagId));
            }
        }


        [Test]
        public async void Check05UpdateSetupOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateSetupServiceAsync<Post, SimplePostDtoAsync>(db);
                var firstPost = db.Posts.Include(x => x.Tags).AsNoTracking().First();

                //ATTEMPT
                var dto = await service.GetOriginalAsync(firstPost.PostId);
                dto.LogSpecificName("End");

                //VERIFY
                dto.PostId.ShouldEqual(firstPost.PostId);
                dto.BloggerName.ShouldEqual(firstPost.Blogger.Name);
                dto.Title.ShouldEqual(firstPost.Title);
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.TagId), dto.Tags.Select(x => x.TagId));
            }
        }


        [Test]
        public async void Check06UpdateWithListDtoOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var firstPost = db.Posts.Include(x => x.Tags).AsNoTracking().First();
                var service = new UpdateServiceAsync<Post, SimplePostDtoAsync>(db);
                var setupService = new UpdateSetupServiceAsync<Post, SimplePostDtoAsync>(db);

                //ATTEMPT
                var dto = await setupService.GetOriginalAsync(firstPost.PostId);
                dto.Title = Guid.NewGuid().ToString();
                var status = await service.UpdateAsync(dto);
                dto.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.SuccessMessage.ShouldEqual("Successfully updated Post.");
                snap.CheckSnapShot(db);
                
            }
        }

        [Test]
        public async void Check07UpdateWithListDtoCorrectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var firstPost = db.Posts.Include(x => x.Tags).AsNoTracking().First();
                var service = new UpdateServiceAsync<Post, SimplePostDtoAsync>(db);
                var setupService = new UpdateSetupServiceAsync<Post, SimplePostDtoAsync>(db);

                //ATTEMPT
                var dto = await setupService.GetOriginalAsync(firstPost.PostId);
                dto.Title = Guid.NewGuid().ToString();
                var status = await service.UpdateAsync(dto);
                dto.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                var updatedPost = db.Posts.Include(x => x.Tags).First();
                updatedPost.Title.ShouldEqual(dto.Title);
                updatedPost.Blogger.ShouldNotEqualNull();
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.TagId), updatedPost.Tags.Select(x => x.TagId));

            }
        }

        [Test]
        public async void Check08UpdateWithListDtoBad()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var firstPost = db.Posts.Include(x => x.Tags).AsNoTracking().First();
                var service = new UpdateServiceAsync<Post, SimplePostDtoAsync>(db);
                var setupService = new UpdateSetupServiceAsync<Post, SimplePostDtoAsync>(db);

                //ATTEMPT
                var dto = await setupService.GetOriginalAsync(firstPost.PostId);
                dto.Title = "Can't I ask a question?";
                var status = await service.UpdateAsync(dto);
                dto.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("Sorry, but you can't ask a question, i.e. the title can't end with '?'.");

            }
        }

        [Test]
        public async void Check08CreateWithListDtoBad()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateServiceAsync<Post, SimplePostDtoAsync>(db);

                //ATTEMPT
                var dto = new SimplePostDtoAsync();
                var status = await service.CreateAsync(dto);
                dto.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("Create of a new Post is not supported in this mode.");

            }
        }

    }
}
