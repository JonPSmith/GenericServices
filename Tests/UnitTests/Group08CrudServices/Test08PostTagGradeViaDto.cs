#region licence
// The MIT License (MIT)
// 
// Filename: Test08PostTagGradeViaDto.cs
// Date Created: 2014/05/27
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
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
                var list = service.GetAll().ToList();

                //VERIFY
                list.Count().ShouldEqual(2);
                list.First().PostPartTitle.ShouldEqual(firstGrade.PostPart.Title);
                list.First().TagPartName.ShouldEqual(firstGrade.TagPart.Name);

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
