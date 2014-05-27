using System;
using System.Data.Entity;
using System.Linq;
using GenericServices;
using GenericServices.Services;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group04Services
{
    class Test02APostTagGradeDirect
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
        public void Check01PostTagGradeDeleteOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var service = new DeleteService<PostTagGrade>(db);
                var firstPtgUntracked = db.PostTagGrades.AsNoTracking().First();

                //ATTEMPT
                var status = service.Delete(firstPtgUntracked.PostId, firstPtgUntracked.TagId);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, 0,0,0,0,-1);

            }
        }

    }
}
