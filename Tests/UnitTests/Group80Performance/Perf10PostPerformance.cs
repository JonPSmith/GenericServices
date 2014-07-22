using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group80Performance
{
    [Ignore]
    class Perf10PostPerformance
    {

        private int _firstPostId;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            DataLayerInitialise.InitialiseThis();
        }

        [Test]
        public void Perf01NPostDatabaseOk()
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

        [Test]
        public void Perf02NAllGenericAgainstEfOk()
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
        public void Perf02NAllSelfSelectAgainstEfOk()
        {
            new SimplePostDto().CacheSetup();
            new DetailPostDto().CacheSetup();

            Console.WriteLine("Testing with N of each class in database");
            RunEfPerformanceTests(10, ResetDatabaseNAll);
            RunGSelectPerformanceTests(10, ResetDatabaseNAll);
            Console.WriteLine("----------------------------------------");
            RunEfPerformanceTests(100, ResetDatabaseNAll);
            RunGSelectPerformanceTests(100, ResetDatabaseNAll);
        }

        [Test]
        public async void Perf10NAllDatabaseAsyncOk()
        {
            new SimplePostDto().CacheSetup();
            new DetailPostDto().CacheSetup();

            Console.WriteLine("Testing async with N of each class in database");
            await Task.WhenAll(RunEfPerformanceTestsAsync(10, ResetDatabaseNAll));
            await Task.WhenAll(RunGenericPerformanceTestsAsync(10, ResetDatabaseNAll));
            Console.WriteLine("----------------------------------------");
            await Task.WhenAll(RunEfPerformanceTestsAsync(100, ResetDatabaseNAll));
            await Task.WhenAll(RunGenericPerformanceTestsAsync(100, ResetDatabaseNAll));
        }

        [Test]
        public async void Perf20NCompareEfSyncAndAsyncOk()
        {
            new SimplePostDto().CacheSetup();
            new DetailPostDto().CacheSetup();

            Console.WriteLine("Compare Ef access, sync and Async");
            RunEfPerformanceTests(10, ResetDatabaseNAll);
            await Task.WhenAll(RunEfPerformanceTestsAsync(10, ResetDatabaseNAll));
            Console.WriteLine("----------------------------------------");
            RunEfPerformanceTests(100, ResetDatabaseNAll);
            await Task.WhenAll(RunEfPerformanceTestsAsync(100, ResetDatabaseNAll));
        }

        [Test]
        public async void Perf21NCompareEfSyncAndAsync1000Ok()
        {
            new SimplePostDto().CacheSetup();
            new DetailPostDto().CacheSetup();

            Console.WriteLine("Compare Ef access, sync and Async");
            RunEfPerformanceTests(10, ResetDatabaseNAll);
            await Task.WhenAll(RunEfPerformanceTestsAsync(10, ResetDatabaseNAll));
            Console.WriteLine("----------------------------------------");
            RunEfPerformanceTests(1000, ResetDatabaseNAll);
            await Task.WhenAll(RunEfPerformanceTestsAsync(1000, ResetDatabaseNAll));
        }

        [Test]
        public async void Perf25NCompareGenericSyncAndAsyncOk()
        {
            new SimplePostDto().CacheSetup();
            new DetailPostDto().CacheSetup();

            Console.WriteLine("Compare Generic access, sync and Async");
            RunGenericPerformanceTests(10, ResetDatabaseNAll);
            await Task.WhenAll(RunGenericPerformanceTestsAsync(10, ResetDatabaseNAll));
            Console.WriteLine("----------------------------------------");
            RunGenericPerformanceTests(100, ResetDatabaseNAll);
            await Task.WhenAll(RunGenericPerformanceTestsAsync(100, ResetDatabaseNAll));
        }

        //-----------------------------------
        //EF test groups

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

        private async Task RunEfPerformanceTestsAsync(int numInDatabase, Func<int, int> clearDatabaseAction)
        {
            Console.WriteLine("EF, with {0} in database -----------------------", numInDatabase);
            _firstPostId = clearDatabaseAction(numInDatabase);
            await Task.WhenAll(RunTestAsync(numInDatabase, "Async List all, Ef Direct", DatabaseHelpers.ListEfDirectAsync<Post>));
            await Task.WhenAll(RunTestAsync(numInDatabase, "Async List all, Ef Dto", DatabaseHelpers.ListPostEfViaDtoAsync));
            await Task.WhenAll(RunTestAsync(numInDatabase, "Async Create, Ef Direct", DatabaseHelpers.CreatePostEfDirectAsync));
            await Task.WhenAll(RunTestAsync(numInDatabase, "Async Update, Ef Direct", DatabaseHelpers.UpdatePostEfDirectAsync));
            await Task.WhenAll(RunTestAsync(numInDatabase, "Async Delete, Ef Direct", DatabaseHelpers.DeletePostEfDirectAsync));
        }

        //-----------------------------------
        //Generic test groups

        private async Task RunGenericPerformanceTestsAsync(int numInDatabase, Func<int, int> clearDatabaseAction)
        {
            Console.WriteLine("Generic, with {0} in database -----------------------", numInDatabase);
            _firstPostId = clearDatabaseAction(numInDatabase);

            await Task.WhenAll(RunTestAsync(numInDatabase, "Async List all, Generic Direct", DatabaseHelpers.ListGenericDirectAsync<Post>));
            await Task.WhenAll(RunTestAsync(numInDatabase, "Async List all, Generic Dto", DatabaseHelpers.ListPostGenericViaDtoAsync));
            await Task.WhenAll(RunTestAsync(numInDatabase, "Async Create, Generic Direct", DatabaseHelpers.CreatePostGenericDirectAsync));
            await Task.WhenAll(RunTestAsync(numInDatabase, "Async Update, Generic Direct", DatabaseHelpers.UpdatePostGenericDirectAsync));
            await Task.WhenAll(RunTestAsync(numInDatabase, "Async Update, Generic Dto", DatabaseHelpers.UpdatePostGenericViaDtoAsync));
            await Task.WhenAll(RunTestAsync(numInDatabase, "Async Delete, Generic Direct", DatabaseHelpers.DeletePostGenericDirectAsync));
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

        //---------------------------------------------
        //Generic, self-select tets groups

        private void RunGSelectPerformanceTests(int numInDatabase, Func<int, int> clearDatabaseAction)
        {
            Console.WriteLine("Generic with self-select, with {0} in database -----------------------", numInDatabase);
            _firstPostId = clearDatabaseAction(numInDatabase);

            RunTest(numInDatabase, "List all, GSelect Direct", DatabaseHelpers.ListGSelectDirect<Post>);
            RunTest(numInDatabase, "List all, GSelect Dto", DatabaseHelpers.ListPostGSelectViaDto);
            RunTest(numInDatabase, "Create, GSelect Direct", DatabaseHelpers.CreatePostGSelectDirect);
            RunTest(numInDatabase, "Update, GSelect Direct", DatabaseHelpers.UpdatePostGSelectDirect);
            RunTest(numInDatabase, "Update, GSelect Dto", DatabaseHelpers.UpdatePostGSelectViaDto);
            RunTest(numInDatabase, "Delete, Generic Direct", DatabaseHelpers.DeletePostGenericDirect);
        }

        private async Task RunGSelectPerformanceTestsAsync(int numInDatabase, Func<int, int> clearDatabaseAction)
        {
            Console.WriteLine("Generic with self-select, with {0} in database -----------------------", numInDatabase);
            _firstPostId = clearDatabaseAction(numInDatabase);

            await Task.WhenAll(RunTestAsync(numInDatabase, "Async List all, GSelect Direct", DatabaseHelpers.ListGSelectDirectAsync<Post>));
            await Task.WhenAll(RunTestAsync(numInDatabase, "Async List all, GSelect Dto", DatabaseHelpers.ListPostGSelectViaDtoAsync));
            await Task.WhenAll(RunTestAsync(numInDatabase, "Async Create, GSelect Direct", DatabaseHelpers.CreatePostGSelectDirectAsync));
            await Task.WhenAll(RunTestAsync(numInDatabase, "Async Update, GSelect Direct", DatabaseHelpers.UpdatePostGSelectDirectAsync));
            await Task.WhenAll(RunTestAsync(numInDatabase, "Async Update, GSelect Dto", DatabaseHelpers.UpdatePostGSelectViaDtoAsync));
            await Task.WhenAll(RunTestAsync(numInDatabase, "Async Delete, Generic Direct", DatabaseHelpers.DeletePostGenericDirectAsync));
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

        private async Task RunTestAsync(int numCyclesToRun, string testType, Func<SampleWebAppDb, int, Task> actionToRun)
        {
            var timer = new Stopwatch();
            timer.Start();
            using (var db = new SampleWebAppDb())
            {
                for (int i = 0; i < numCyclesToRun; i++)
                {
                    await actionToRun(db, i + _firstPostId);//.ConfigureAwait(false);
                }
            }
            timer.Stop();
            Console.WriteLine("Ran {0}: total time = {1} ms ({2:f1} ms per action)", testType,
                timer.ElapsedMilliseconds,
                timer.ElapsedMilliseconds / ((double)numCyclesToRun));
        }
    }
}
