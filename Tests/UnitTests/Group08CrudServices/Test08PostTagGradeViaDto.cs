using System.Data.Entity;
using System.Linq;
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
    class Test08PostTagGradeViaDto
    {

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            new SimplePostTagGradeDto().CacheSetup();
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
        public void Check01ListGradesOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new ListService<PostTagGrade, SimplePostTagGradeDto>(db);
                var firstGrade = db.PostTagGrades.Include(x => x.TagPart).Include(x => x.PostPart).First();

                //ATTEMPT
                var status = service.GetMany().TryManyWithPermissionChecking();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.Count().ShouldEqual(2);
                status.Result.First().PostPartTitle.ShouldEqual(firstGrade.PostPart.Title);
                status.Result.First().TagPartName.ShouldEqual(firstGrade.TagPart.Name);

            }
        }

        [Test]
        public void Check02DetailOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailService<PostTagGrade, SimplePostTagGradeDto>(db);
                var firstGrade = db.PostTagGrades.Include(x => x.TagPart).Include(x => x.PostPart).First();

                //ATTEMPT
                var status = service.GetDetailUsingWhere(x => x.PostId == firstGrade.PostId && x.TagId == firstGrade.TagId);
                status.Result.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.PostId.ShouldEqual(firstGrade.PostId);
                status.Result.TagPartName.ShouldEqual(firstGrade.TagPart.Name);
                status.Result.PostPartTitle.ShouldEqual(firstGrade.PostPart.Title);
            }
        }

        [Test]
        public void Check05UpdateSetupOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateSetupService<PostTagGrade, SimplePostTagGradeDto>(db);
                var firstGrade = db.PostTagGrades.Include(x => x.TagPart).Include(x => x.PostPart).First();

                //ATTEMPT
                var status = service.GetOriginal(firstGrade.PostId, firstGrade.TagId);
                status.Result.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.PostId.ShouldEqual(firstGrade.PostId);
                status.Result.TagPartName.ShouldEqual(firstGrade.TagPart.Name);
                status.Result.PostPartTitle.ShouldEqual(firstGrade.PostPart.Title);
            }
        }


        [Test]
        public void Check06UpdateOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var firstGrade = db.PostTagGrades.Include(x => x.TagPart).Include(x => x.PostPart).First();
                var service = new UpdateService<PostTagGrade, SimplePostTagGradeDto>(db);
                var setupService = new UpdateSetupService<PostTagGrade, SimplePostTagGradeDto>(db);

                //ATTEMPT
                var setupStatus = setupService.GetOriginal(firstGrade.PostId, firstGrade.TagId);
                setupStatus.IsValid.ShouldEqual(true, setupStatus.Errors);
                setupStatus.Result.Grade = 999;
                var status = service.Update(setupStatus.Result);
                setupStatus.Result.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.SuccessMessage.ShouldEqual("Successfully updated PostTagGrade.");
                snap.CheckSnapShot(db);
                var updatedfirstGrade = db.PostTagGrades.First();
                updatedfirstGrade.Grade.ShouldEqual(999);
            }
        }

    }
}
