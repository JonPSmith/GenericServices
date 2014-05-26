using System.Linq;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.Helpers;
using System.Data.Entity;

namespace Tests.UnitTests.Group01DataClasses
{
    class Tests01Setup
    {
        [Test]
        public void Check01DatabaseResetOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                DataLayerInitialise.InitialiseThis();
                var filepath = TestFileHelpers.GetTestFileFilePath("DbContentSimple.xml");

                //ATTEMPT
                DataLayerInitialise.ResetDatabaseToTestData(db, filepath);

                //VERIFY
                db.Blogs.Count().ShouldEqual(2);
                db.Posts.Count().ShouldEqual(3);
                db.Tags.Count().ShouldEqual(3);
                db.Database.SqlQuery<int>("SELECT COUNT(*) FROM dbo.TagPosts").First().ShouldEqual(5);
                db.PostTagGrades.Count().ShouldEqual(1);
            }
        }

        [Test]
        public void Check02DatabaseDataLinksOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                DataLayerInitialise.InitialiseThis();
                var filepath = TestFileHelpers.GetTestFileFilePath("DbContentSimple.xml");

                //ATTEMPT
                DataLayerInitialise.ResetDatabaseToTestData(db, filepath);

                //VERIFY
                var allPosts = db.Posts.Include(x => x.Blogger).Include(x => x.Tags).ToList();
                allPosts[0].Blogger.Name.ShouldEqual("Fred Bloggs");
                string.Join(",", allPosts[0].Tags.Select(x => x.Slug)).ShouldEqual("ugly,bad");
                allPosts[1].Blogger.Name.ShouldEqual("Jon Smith");
                string.Join(",", allPosts[1].Tags.Select(x => x.Slug)).ShouldEqual("good,ugly");
                allPosts[2].Blogger.Name.ShouldEqual("Jon Smith");
                string.Join(",", allPosts[2].Tags.Select(x => x.Slug)).ShouldEqual("bad");

                db.PostTagGrades.Count().ShouldEqual(1);
                db.PostTagGrades.Single().PostId.ShouldEqual(allPosts[0].PostId);
                db.PostTagGrades.Include(x => x.TagPart).Single().TagPart.Slug.ShouldEqual("bad");
            }
        }

    }
}
