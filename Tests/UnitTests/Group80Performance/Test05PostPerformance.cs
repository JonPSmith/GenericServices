using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group80Performance
{
    [Ignore]
    class Test05PostPerformance
    {


        private int _firstPostId;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            using (var db = new SampleWebAppDb())
            {
                DataLayerInitialise.InitialiseThis();
            }
        }

        [Test]
        public void Perf01NAllDatabaseOk()
        {
            new SimplePostDto().CacheSetup();
            new DetailPostDto().CacheSetup();

            Console.WriteLine("Testing with N of each class in database");
            RunEfPerformanceTests(10, ResetDatabaseNAll);
            RunGenericPerformanceTests(10, ResetDatabaseNAll);
            Console.WriteLine("----------------------------------------");
            RunEfPerformanceTests(100, ResetDatabaseNAll);
            RunGenericPerformanceTests(100, ResetDatabaseNAll);

        }

        [Test]
        public void Perf02NPostDatabaseOk()
        {
            new SimplePostDto().CacheSetup();
            new DetailPostDto().CacheSetup();

            Console.WriteLine("Testing with N of post, and two tag, two bloggers in database");
            RunEfPerformanceTests(10, ResetDatabaseNPost);
            RunGenericPerformanceTests(10, ResetDatabaseNPost);
            Console.WriteLine("----------------------------------------");
            RunEfPerformanceTests(100, ResetDatabaseNPost);
            RunGenericPerformanceTests(100, ResetDatabaseNPost);

        }

        private void RunGenericPerformanceTests(int numInDatabase, Func<int,int> clearDatabaseAction)
        {
            Console.WriteLine("Generic, with {0} in database -----------------------", numInDatabase);
            _firstPostId = clearDatabaseAction(numInDatabase);

            RunTest(numInDatabase, "List all, Generic Direct", DatabaseHelpers.ListGenericDirect<Post>);
            RunTest(numInDatabase, "List all, Generic Dto", DatabaseHelpers.ListPostGenericViaDto);
            RunTest(numInDatabase, "Create, Generic Direct", DatabaseHelpers.CreatePostGenericDirect);
            RunTest(numInDatabase, "Update, Generic Direct", DatabaseHelpers.UpdatePostGenericDirect);
            RunTest(numInDatabase, "Update, Generic Dto", DatabaseHelpers.UpdatePostGenericViaDto);
            RunTest(numInDatabase, "Delete, Generic Direct", DatabaseHelpers.DeletePostGenericDirect);
        }

        private void RunEfPerformanceTests(int numInDatabase, Func<int, int> clearDatabaseAction)
        {
            Console.WriteLine("EF, with {0} in database -----------------------", numInDatabase);
            _firstPostId = clearDatabaseAction(numInDatabase);
            RunTest(numInDatabase, "List all, Ef Direct", DatabaseHelpers.ListEfDirect<Post>);
            RunTest(numInDatabase, "List all, Ef Dto", DatabaseHelpers.ListPostEfViaDto);
            RunTest(numInDatabase, "Create, Ef Direct", DatabaseHelpers.CreatePostEfDirect);
            RunTest(numInDatabase, "Update, Ef Direct", DatabaseHelpers.UpdatePostEfDirect);
            RunTest(numInDatabase, "Delete, Ef Direct", DatabaseHelpers.DeletePostEfDirect);
        }

        private static int ResetDatabaseNAll(int numToPutInDatabase)
        {
            using (var db = new SampleWebAppDb())
            {
                db.FillComputedNAll(numToPutInDatabase);
                return db.Posts.Min(x => x.PostId);
            }
        }

        private static int ResetDatabaseNPost(int numToPutInDatabase)
        {
            using (var db = new SampleWebAppDb())
            {
                db.FillComputedNPost(numToPutInDatabase);
                return db.Posts.Min(x => x.PostId);
            }
        }
        private void RunTest(int numCyclesToRun, string testType, Action<SampleWebAppDb, int> actionToRun)
        {
            var timer = new Stopwatch();
            timer.Start();
            using (var db = new SampleWebAppDb())
            {
                for (int i = 0; i < numCyclesToRun; i++)
                {
                    actionToRun(db, i + _firstPostId);
                }
            }
            timer.Stop();
            Console.WriteLine("Ran {0}: total time = {1} ms ({2:f1} ms per action)", testType,
                timer.ElapsedMilliseconds,
                timer.ElapsedMilliseconds / ((double)numCyclesToRun));
        }
    }
}
