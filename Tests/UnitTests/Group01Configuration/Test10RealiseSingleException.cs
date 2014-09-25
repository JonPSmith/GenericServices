using System;
using System.Linq;
using System.Threading.Tasks;
using GenericServices;
using GenericServices.Core;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.Helpers;

namespace Tests.UnitTests.Group01Configuration
{
    class Test10RealiseSingleException
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
        public void Test01NoSingleExceptionSetOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var id = db.Posts.First().PostId;
                ServicesConfiguration.RealiseSingleExceptionMethod = null;

                //ATTEMPT
                var status = db.Posts.Where(x => x.PostId == id).RealiseSingleWithErrorChecking();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
            }
        }

        [Test]
        public void Test02NoSingleExceptionSetBad()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                ServicesConfiguration.RealiseSingleExceptionMethod = null;

                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => db.Posts.RealiseSingleWithErrorChecking());

                //VERIFY
                ex.Message.ShouldEqual("Sequence contains more than one element");
            }
        }

        private string TestExceptionCatch(Exception ex, string callingMethodName)
        {
            if (ex is InvalidOperationException)
                return string.Format("{0} threw error {1}", callingMethodName, ex.Message);
            
            return null;
        }

        private string TestExceptionNoCatch(Exception ex, string callingMethodName)
        {
            return null;
        }

        [Test]
        public void Test05HadSingleExceptionSetCatchOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var id = db.Posts.First().PostId;
                ServicesConfiguration.RealiseSingleExceptionMethod = TestExceptionCatch;

                //ATTEMPT
                var status = db.Posts.RealiseSingleWithErrorChecking();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("Test05HadSingleExceptionSetCatchOk threw error Sequence contains more than one element");
            }
        }

        [Test]
        public void Test06HadSingleExceptionNoCatchBad()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                ServicesConfiguration.RealiseSingleExceptionMethod = TestExceptionNoCatch;

                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => db.Posts.RealiseSingleWithErrorChecking());

                //VERIFY
                ex.Message.ShouldEqual("Sequence contains more than one element");
            }
        }

        [Test]
        public async Task Test10HadSingleExceptionSetCatchOkAsync()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var id = db.Posts.First().PostId;
                ServicesConfiguration.RealiseSingleExceptionMethod = TestExceptionCatch;

                //ATTEMPT
                var status = await db.Posts.RealiseSingleWithErrorCheckingAsync();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEndWith(" threw error Sequence contains more than one element");
            }
        }

        [Test]
        public async Task Test11HadSingleExceptionNoCatchBadAsync()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                ServicesConfiguration.RealiseSingleExceptionMethod = TestExceptionNoCatch;

                //ATTEMPT
                try
                {
                    await db.Posts.RealiseSingleWithErrorCheckingAsync();
                }
                catch (InvalidOperationException ex)
                {
                    ex.Message.ShouldEqual("Sequence contains more than one element");
                    return;
                }

                //VERIFY
                Assert.Fail("Should have thrown an exception");
            }
        }
    }
}
