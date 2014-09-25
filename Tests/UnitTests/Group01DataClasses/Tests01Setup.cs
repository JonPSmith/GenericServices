#region licence
// The MIT License (MIT)
// 
// Filename: Tests01Setup.cs
// Date Created: 2014/05/19
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
using System.Linq;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.Helpers;
using System.Data.Entity;

namespace Tests.UnitTests.Group01DataClasses
{
    class Tests01Setup
    {
        [Test]
        public void Check01DatabaseResetOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                DataLayerInitialise.InitialiseThis();
                var filepath = TestFileHelpers.GetTestFileFilePath("DbContentSimple.xml");

                //ATTEMPT
                DataLayerInitialise.ResetDatabaseToTestData(db, filepath);

                //VERIFY
                db.Blogs.Count().ShouldEqual(2);
                db.Posts.Count().ShouldEqual(3);
                db.Tags.Count().ShouldEqual(3);
                db.Database.SqlQuery<int>("SELECT COUNT(*) FROM dbo.TagPosts").First().ShouldEqual(5);
                db.PostTagGrades.Count().ShouldEqual(2);
            }
        }

        [Test]
        public void Check02DatabaseDataLinksOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                DataLayerInitialise.InitialiseThis();
                var filepath = TestFileHelpers.GetTestFileFilePath("DbContentSimple.xml");

                //ATTEMPT
                DataLayerInitialise.ResetDatabaseToTestData(db, filepath);

                //VERIFY
                var allPosts = db.Posts.Include(x => x.Blogger).Include(x => x.Tags).ToList();
                allPosts[0].Blogger.Name.ShouldEqual("Fred Bloggs");
                string.Join(",", allPosts[0].Tags.Select(x => x.Slug)).ShouldEqual("ugly,bad");
                allPosts[1].Blogger.Name.ShouldEqual("Jon Smith");
                string.Join(",", allPosts[1].Tags.Select(x => x.Slug)).ShouldEqual("good,ugly");
                allPosts[2].Blogger.Name.ShouldEqual("Jon Smith");
                string.Join(",", allPosts[2].Tags.Select(x => x.Slug)).ShouldEqual("bad");

                db.PostTagGrades.Count().ShouldEqual(2);
                db.PostTagGrades.ToList().All( x => x.PostId == allPosts[0].PostId).ShouldEqual(true);
                string.Join(",", db.PostTagGrades.Include(x => x.TagPart).Select(x => x.TagPart.Slug)).ShouldEqual("bad,ugly");
            }
        }

    }
}
