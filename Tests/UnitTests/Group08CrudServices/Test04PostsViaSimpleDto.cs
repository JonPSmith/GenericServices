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
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group08CrudServices
{
    class Test04PostsViaSimpleDto
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
            new SimplePostDto().CacheSetup();
        }

        [Test]
        public void Check01ReferenceOk()
        {

            //SETUP    

            //ATTEMPT
            ICreateService<Post, SimplePostDto> createService = new CreateService<Post, SimplePostDto>(null);
            IDetailService<Post, SimplePostDto> detailService = new DetailService<Post, SimplePostDto>(null);
            IListService<Post, SimplePostDto> listService = new ListService<Post, SimplePostDto>(null);
            IUpdateService<Post, SimplePostDto> updateService = new UpdateService<Post, SimplePostDto>(null);

            //VERIFY
            (listService is IListService<Post, SimplePostDto>).ShouldEqual(true);
        }
        
        //--------------------------------------------------------

        [Test]
        public void Check02ListPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new ListService<Post, SimplePostDto>(db);

                //ATTEMPT
                var status = service.GetMany().TryManyWithPermissionChecking();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.Count().ShouldEqual(3);
                status.Result.First().Title.ShouldEqual("First great post");
                status.Result.First().BloggerName.ShouldEqual("Jon Smith");
                status.Result.First().TagNames.ShouldEqual("Ugly post, Good post");
                status.Result.First().LastUpdatedUtc.Kind.ShouldEqual(DateTimeKind.Utc);

            }
        }

        [Test]
        public void Check02DetailPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailService<Post, SimplePostDto>(db);
                var firstPost = db.Posts.Include(x => x.Tags).AsNoTracking().First();

                //ATTEMPT
                var status = service.GetDetail(firstPost.PostId);
                status.Result.LogSpecificName("End");

                //VERIFY
                status.Result.PostId.ShouldEqual(firstPost.PostId);
                status.Result.BloggerName.ShouldEqual(firstPost.Blogger.Name);
                status.Result.Title.ShouldEqual(firstPost.Title);
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.TagId), status.Result.Tags.Select(x => x.TagId));
            }
        }

        [Test]
        public void Check03DetailPostWhereOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailService<Post, SimplePostDto>(db);
                var firstPost = db.Posts.Include(x => x.Tags).AsNoTracking().First();

                //ATTEMPT
                var status = service.GetDetailUsingWhere(x => x.PostId == firstPost.PostId);
                status.Result.LogSpecificName("End");

                //VERIFY
                status.Result.PostId.ShouldEqual(firstPost.PostId);
                status.Result.BloggerName.ShouldEqual(firstPost.Blogger.Name);
                status.Result.Title.ShouldEqual(firstPost.Title);
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.TagId), status.Result.Tags.Select(x => x.TagId));
            }
        }

        [Test]
        public void Check04DetailDirectPostNotFoundBad()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailService<Post, SimplePostDto>(db);

                //ATTEMPT
                var status = service.GetDetail(0);

                //VERIFY
                status.IsValid.ShouldEqual(false, status.Errors);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("We could not find an entry using that filter. Has it been deleted by someone else?");
            }
        }

        [Test]
        public void Check05UpdateSetupOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateSetupService<Post, SimplePostDto>(db);
                var firstPost = db.Posts.Include(x => x.Tags).AsNoTracking().First();

                //ATTEMPT
                var status = service.GetOriginal(firstPost.PostId);
                status.Result.LogSpecificName("End");

                //VERIFY
                status.Result.PostId.ShouldEqual(firstPost.PostId);
                status.Result.BloggerName.ShouldEqual(firstPost.Blogger.Name);
                status.Result.Title.ShouldEqual(firstPost.Title);
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.TagId), status.Result.Tags.Select(x => x.TagId));
            }
        }


        [Test]
        public void Check06UpdateWithListDtoOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var firstPost = db.Posts.Include(x => x.Tags).AsNoTracking().First();
                var service = new UpdateService<Post, SimplePostDto>(db);
                var setupService = new UpdateSetupService<Post, SimplePostDto>(db);

                //ATTEMPT
                var setupStatus = setupService.GetOriginal(firstPost.PostId);
                setupStatus.Result.Title = Guid.NewGuid().ToString();
                var status = service.Update(setupStatus.Result);
                setupStatus.Result.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.SuccessMessage.ShouldEqual("Successfully updated Post.");
                snap.CheckSnapShot(db);
                
            }
        }

        [Test]
        public void Check07UpdateWithListDtoCorrectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var firstPost = db.Posts.Include(x => x.Tags).AsNoTracking().First();
                var service = new UpdateService<Post, SimplePostDto>(db);
                var setupService = new UpdateSetupService<Post, SimplePostDto>(db);

                //ATTEMPT
                var setupStatus = setupService.GetOriginal(firstPost.PostId);
                setupStatus.Result.Title = Guid.NewGuid().ToString();
                var status = service.Update(setupStatus.Result);
                setupStatus.Result.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                var updatedPost = db.Posts.Include(x => x.Tags).First();
                updatedPost.Title.ShouldEqual(setupStatus.Result.Title);
                updatedPost.Blogger.ShouldNotEqualNull();
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.TagId), updatedPost.Tags.Select(x => x.TagId));

            }
        }

        [Test]
        public void Check08UpdateWithListDtoBad()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var listService = new ListService<Post, SimplePostDto>(db);
                var firstPostUntracked = listService.GetMany().First();
                var service = new UpdateService<Post, SimplePostDto>(db);

                //ATTEMPT
                firstPostUntracked.Title = "Can't I ask a question?";
                var status = service.Update(firstPostUntracked);
                firstPostUntracked.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("Sorry, but you can't ask a question, i.e. the title can't end with '?'.");

            }
        }

        [Test]
        public void Check08CreateWithListDtoBad()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateService<Post, SimplePostDto>(db);

                //ATTEMPT
                var dto = new SimplePostDto();
                var status = service.Create(dto);
                dto.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("Create of a new Post is not supported in this mode.");

            }
        }

    }
}
