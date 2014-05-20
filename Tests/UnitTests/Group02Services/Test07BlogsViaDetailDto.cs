using System.Data.Entity;
using System.Linq;
using GenericServices;
using GenericServices.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group02Services
{
    class Test07BlogsViaDetailDto
    {

        [SetUp]
        public void SetUp()
        {
            using (var db = new SampleWebAppDb())
            {
                DataLayerInitialise.InitialiseThis();
                DataLayerInitialise.ResetDatabaseToTestData(db);
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
                var query = service.GetList();
                var list = query.ToList();

                //VERIFY
                list.Count.ShouldEqual(2);
                var firstBlog = db.Blogs.Include(x => x.Posts).AsNoTracking().First();
                list[0].Name.ShouldEqual(firstBlog.Name);
                list[0].EmailAddress.ShouldEqual(firstBlog.EmailAddress);
                list[0].Posts.ShouldNotEqualNull();
                CollectionAssert.AreEquivalent(firstBlog.Posts.Select(x => x.PostId), list[0].Posts.Select(x => x.PostId));
            }
        }

    }
}
