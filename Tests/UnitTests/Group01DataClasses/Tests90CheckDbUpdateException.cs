using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group01DataClasses
{
    class Tests90CheckDbUpdateException
    {

        [SetUp]
        public void SetUp()
        {
            using (var db = new SampleWebAppDb())
            {
                DataLayerInitialise.InitialiseThis();
                var filepath = TestFileHelpers.GetTestFileFilePath("DbContentSimple.xml");
                DataLayerInitialise.ResetDatabaseToTestData(db, filepath);
                db.PostTagGrades.RemoveRange(db.PostTagGrades);
                db.SaveChanges();
            }
        }

        [Test]
        public void Check01DeleteFailBecauseOfForeignKeyOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var post = db.Posts.First();
                db.PostLinks.Add(new PostLink {PostPart = post});
                db.SaveChanges();
            }
            
            using (var db = new SampleWebAppDb())
            {
                //ATTEMPT
                db.Posts.Remove(db.Posts.First());
                var status = db.SaveChangesWithValidation();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
            }
        }


        [Test]
        public void Check02UniqueKeyErrorOk()
        {
            //NOTE: To test this I needed to comment out the ValidateEntity method in SampleWebAppDb

            var tagGuid = Guid.NewGuid().ToString("N");
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                db.Tags.Add(new Tag { Name = tagGuid, Slug = tagGuid});
                db.SaveChanges();
            }

            using (var db = new SampleWebAppDb())
            {
                //ATTEMPT
                db.Tags.Add(new Tag { Name = tagGuid, Slug = tagGuid });
                var status = db.SaveChangesWithValidation();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
            }
        }


        [Test]
        public void Check05CauseBothErrorsOk()
        {
            //NOTE: To test this I needed to comment out the ValidateEntity method in SampleWebAppDb

            var tagGuid = Guid.NewGuid().ToString("N");
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var post = db.Posts.First();
                db.PostLinks.Add(new PostLink { PostPart = post });
                db.Tags.Add(new Tag { Name = tagGuid, Slug = tagGuid });
                db.SaveChanges();
            }

            using (var db = new SampleWebAppDb())
            {
                //ATTEMPT
                db.Posts.Remove(db.Posts.First());
                db.Tags.Add(new Tag { Name = tagGuid, Slug = tagGuid });
                var status = db.SaveChangesWithValidation();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);         //for these two cases we only get one error
            }
        }
    }
}
