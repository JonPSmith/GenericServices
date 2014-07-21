using System.Data.Entity;
using System.Linq;
using GenericServices.Services;
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
                var query = service.GetList();
                var list = query.ToList();

                //VERIFY
                list.Count.ShouldEqual(2);
                list[0].PostPartTitle.ShouldEqual(firstGrade.PostPart.Title);
                list[0].TagPartName.ShouldEqual(firstGrade.TagPart.Name);

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
                var dto = service.GetDetailUsingWhere(x => x.PostId == firstGrade.PostId && x.TagId == firstGrade.TagId);
                dto.LogSpecificName("End");

                //VERIFY
                dto.PostId.ShouldEqual(firstGrade.PostId);
                dto.TagPartName.ShouldEqual(firstGrade.TagPart.Name);
                dto.PostPartTitle.ShouldEqual(firstGrade.PostPart.Title);
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
                var dto = service.GetOriginal(firstGrade.PostId, firstGrade.TagId);
                dto.LogSpecificName("End");

                //VERIFY
                dto.PostId.ShouldEqual(firstGrade.PostId);
                dto.TagPartName.ShouldEqual(firstGrade.TagPart.Name);
                dto.PostPartTitle.ShouldEqual(firstGrade.PostPart.Title);
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
                var dto = setupService.GetOriginal(firstGrade.PostId, firstGrade.TagId);
                dto.Grade = 999;
                var status = service.Update(dto);
                dto.LogSpecificName("End");

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
