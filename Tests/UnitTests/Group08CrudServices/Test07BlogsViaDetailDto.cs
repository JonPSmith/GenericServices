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
    class Test07BlogsViaDetailDto
    {

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            new SimpleBlogWithPostsDto().CacheSetup();
        }

        [SetUp]
        public void SetUp()
        {
            using (var db = new SampleWebAppDb())
            {
                DataLayerInitialise.InitialiseThis();
                var filepath = TestFileHelpers.GetTestFileFilePath("DbContentSimple.xml");
                DataLayerInitialise.ResetDatabaseToTestData(db, filepath);
            }
        }


        [Test]
        public void Check01ReferencesOk()
        {

            //SETUP    

            //ATTEMPT
            IListService<Blog, SimpleBlogWithPostsDto> listService = new ListService<Blog, SimpleBlogWithPostsDto>(null);

            //VERIFY
            (listService is ListService<Blog, SimpleBlogWithPostsDto>).ShouldEqual(true);
        }

        [Test]
        public void Check02ListBlogsCheckIncludesPostsOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new ListService<Blog, SimpleBlogWithPostsDto>(db);

                //ATTEMPT
                var list = service.GetAll().ToList();

                //VERIFY
                list.Count().ShouldEqual(2);
                var firstBlog = db.Blogs.Include(x => x.Posts).AsNoTracking().First();
                list.First().Name.ShouldEqual(firstBlog.Name);
                list.First().EmailAddress.ShouldEqual(firstBlog.EmailAddress);
                list.First().Posts.ShouldNotEqualNull();
                CollectionAssert.AreEquivalent(firstBlog.Posts.Select(x => x.PostId), list.First().Posts.Select(x => x.PostId));
            }
        }

    }
}
