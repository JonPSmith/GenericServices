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

    }
}
