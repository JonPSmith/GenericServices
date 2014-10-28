using System.Data.Entity;
using System.Linq;
using GenericServices;
using GenericServices.Services.Concrete;
using GenericServices.ServicesAsync.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group08CrudServices
{
    class Test01DelegateDecompiler
    {

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            using (var db = new SampleWebAppDb())
            {
                DataLayerInitialise.InitialiseThis();
                var filepath = TestFileHelpers.GetTestFileFilePath("DbContentSimple.xml");
                DataLayerInitialise.ResetDatabaseToTestData(db, filepath);
            }
        }

        [Test]
        public void Check01NoComputedTakenInDtoDelegateDecomplierTurnedOffOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                GenericServicesConfig.UseDelegateDecompilerWhereNeeded = false;
                var service = new ListService<Post,SimplePostDto>(db);
                var firstPost = db.Posts.Include(x => x.Blogger).First();

                //ATTEMPT
                var list = service.GetAll().OrderBy( x => x.PostId).ToList();

                //VERIFY
                list.Count().ShouldEqual(3);
                list.First().Title.ShouldEqual(firstPost.Title);
                list.First().BloggerName.ShouldEqual(firstPost.Blogger.Name);
                list.First().Tags.ShouldNotEqualNull();
            }
        }

        [Test]
        public void Check02NoComputedTakenInDtoDecomplierOnButNoComputedCopiedOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                GenericServicesConfig.UseDelegateDecompilerWhereNeeded = true;
                var service = new ListService<Post, SimplePostDto>(db);
                var firstPost = db.Posts.Include(x => x.Blogger).First();

                //ATTEMPT
                var list = service.GetAll().OrderBy(x => x.PostId).ToList();

                //VERIFY
                list.Count().ShouldEqual(3);
                list.First().Title.ShouldEqual(firstPost.Title);
                list.First().BloggerName.ShouldEqual(firstPost.Blogger.Name);
                list.First().Tags.ShouldNotEqualNull();
            }
        }

        //------------------------------------------------------
        //now a DTO that reads those computed properties

        [Test]
        public void Check05ListComputedOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                GenericServicesConfig.UseDelegateDecompilerWhereNeeded = true;
                var service = new ListService<Post, DelegateDecompilePostDto>(db);
                var firstPost = db.Posts.Include(x => x.Tags).First();

                //ATTEMPT
                var list = service.GetAll().OrderBy(x => x.PostId).ToList();

                //VERIFY
                list.Count().ShouldEqual(3);
                list.First().Title.ShouldEqual(firstPost.Title);
                list.First().BloggerNameAndEmail.ShouldEndWith("nospam.com)");
                list.First().TagNames.ShouldNotEqualNull();
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.Name), list.First().TagNames); 
            }
        }

        [Test]
        public void Check06DetailComputedOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                GenericServicesConfig.UseDelegateDecompilerWhereNeeded = true;
                var service = new DetailService<Post, DelegateDecompilePostDto>(db);
                var firstPost = db.Posts.Include(x => x.Tags).First();

                //ATTEMPT
                var status = service.GetDetail(firstPost.PostId);

                //VERIFY
                status.ShouldBeValid();
                status.Result.Title.ShouldEqual(firstPost.Title);
                status.Result.BloggerNameAndEmail.ShouldEndWith("nospam.com)");
                status.Result.TagNames.ShouldNotEqualNull();
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.Name), status.Result.TagNames);
            }
        }

        [Test]
        public async void Check07DetailComputedAsyncOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                GenericServicesConfig.UseDelegateDecompilerWhereNeeded = true;
                var service = new DetailServiceAsync<Post, DelegateDecompilePostDtoAsync>(db);
                var firstPost = db.Posts.Include(x => x.Tags).First();

                //ATTEMPT
                var status = await service.GetDetailAsync(firstPost.PostId);

                //VERIFY
                status.ShouldBeValid();
                status.Result.Title.ShouldEqual(firstPost.Title);
                status.Result.BloggerNameAndEmail.ShouldEndWith("nospam.com)");
                status.Result.TagNames.ShouldNotEqualNull();
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.Name), status.Result.TagNames);
            }
        }

    }
}
