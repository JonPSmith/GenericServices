using System.Linq;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group80Performance
{
    class Test01CheckHelpers
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
        public void Check01FillComputedOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                const int numToCreate = 20;
                var snap = new DbSnapShot();        //empty

                //ATTEMPT
                db.FillComputed(numToCreate);

                //VERIFY
                snap.CheckSnapShot(db, numToCreate, numToCreate * 2, numToCreate, numToCreate, numToCreate);
                (db.Posts.Max(x => x.PostId) - db.Posts.Min(x => x.PostId)).ShouldEqual(numToCreate - 1);       //check numbers are consecutive
            }
        }

        [Test]
        public void Check05ListEfDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);

                //ATTEMPT
                var result = db.GetListEfDirect<Post>().ToList();

                //VERIFY
                result.Count.ShouldEqual(snap.NumPosts);
            }
        }

        [Test]
        public void Check06ListEfViaDtoOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);

                //ATTEMPT
                var result = db.GetListPostEfViaDto().ToList();

                //VERIFY
                result.Count.ShouldEqual(snap.NumPosts);
            }
        }

        [Test]
        public void Check06ListEfViaDtoTake5Ok()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);

                //ATTEMPT
                var result = db.GetListPostEfViaDto().Take(5).ToList();

                //VERIFY
                result.Count.ShouldEqual(5);
            }
        }

        [Test]
        public void Check10CreateEfDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);

                //ATTEMPT
                db.CreatePostEfDirect();

                //VERIFY
                db.Posts.Count().ShouldEqual(snap.NumPosts + 1);
            }
        }


        [Test]
        public void Check11UpdateEfDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var id = db.Posts.First().PostId;

                //ATTEMPT
                db.UpdatePostEfDirect(id);

                //VERIFY
                db.Posts.Count().ShouldEqual(snap.NumPosts);
            }
        }

        [Test]
        public void Check12DeleteEfDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var id = db.Posts.AsNoTracking().First().PostId;

                //ATTEMPT
                db.DeletePostEfDirect(id);

                //VERIFY
                db.Posts.Count().ShouldEqual(snap.NumPosts - 1);
            }
        }



    }
}
