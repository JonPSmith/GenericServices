using System;
using System.Data.Entity;
using System.Linq;
using GenericServices;
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
                var query = service.GetList();
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
                var service = new DetailService<Post, SimplePostDto>(db);
                var firstPost = db.Posts.Include(x => x.Tags).AsNoTracking().First();

                //ATTEMPT
                var dto = service.GetDetailUsingWhere(x => x.PostId == firstPost.PostId);
                dto.LogSpecificName("End");

                //VERIFY
                dto.PostId.ShouldEqual(firstPost.PostId);
                dto.BloggerName.ShouldEqual(firstPost.Blogger.Name);
                dto.Title.ShouldEqual(firstPost.Title);
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.TagId), dto.Tags.Select(x => x.TagId));
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
                var dto = service.GetOriginal(firstPost.PostId);
                dto.LogSpecificName("End");

                //VERIFY
                dto.PostId.ShouldEqual(firstPost.PostId);
                dto.BloggerName.ShouldEqual(firstPost.Blogger.Name);
                dto.Title.ShouldEqual(firstPost.Title);
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.TagId), dto.Tags.Select(x => x.TagId));
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
                var dto = setupService.GetOriginal(firstPost.PostId);
                dto.Title = Guid.NewGuid().ToString();
                var status = service.Update(dto);
                dto.LogSpecificName("End");

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
                var dto = setupService.GetOriginal(firstPost.PostId);
                dto.Title = Guid.NewGuid().ToString();
                var status = service.Update(dto);
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
        public void Check08UpdateWithListDtoBad()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var listService = new ListService<Post, SimplePostDto>(db);
                var firstPostUntracked = listService.GetList().First();
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
