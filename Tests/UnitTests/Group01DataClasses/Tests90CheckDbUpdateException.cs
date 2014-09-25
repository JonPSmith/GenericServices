#region licence
// The MIT License (MIT)
// 
// Filename: Tests90CheckDbUpdateException.cs
// Date Created: 2014/07/23
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
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using GenericServices;
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
                db.SaveChanges();
            }
        }

        [Test]
        public void Check01DeleteFailBecauseOfForeignKeyBad()
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
                var status = db.SaveChangesWithChecking();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("This operation failed because another data entry uses this entry.");
            }
        }


        [Test]
        [Ignore("Only run if ValidateEntity in SampleWebAppDb has been commented out")]
        public void Check02UniqueKeyErrorBad()
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
                var status = db.SaveChangesWithChecking();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("One of the properties is marked as Unique index and there is already an entry with that value.");
            }
        }


        [Test]
        [Ignore("Only run if ValidateEntity in SampleWebAppDb has been commented out")]
        public void Check05CauseBothErrorsBad()
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
                var status = db.SaveChangesWithChecking();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);         //for these two cases we only get one error
                status.Errors[0].ErrorMessage.ShouldEqual("One of the properties is marked as Unique index and there is already an entry with that value.");
            }
        }

        //-----------------------------------

        [Test]
        public void Check10UniqueKeyPostTagBad()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var firstPostTag = db.PostTagGrades.Include(x => x.PostPart).Include(x => x.TagPart).First();
          
                //ATTEMPT
                db.PostTagGrades.Add(new PostTagGrade{ PostPart = firstPostTag.PostPart, TagPart = firstPostTag.TagPart });
                var ex = Assert.Throws<DbUpdateException>(  () => db.SaveChangesWithChecking());

                //VERIFY
            }
        }

    }
}
