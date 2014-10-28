#region licence
// The MIT License (MIT)
// 
// Filename: Perf01CheckHelpers.cs
// Date Created: 2014/06/11
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
using Tests.DataClasses.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group80Performance
{
    class Perf01CheckHelpers
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
        public void Check01FillComputedNAllOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                const int numToCreate = 20;
                var snap = new DbSnapShot();        //empty

                //ATTEMPT
                db.FillComputedNAll(numToCreate);

                //VERIFY
                snap.CheckSnapShot(db, numToCreate, numToCreate * 2, numToCreate, numToCreate, numToCreate);
                (db.Posts.Max(x => x.PostId) - db.Posts.Min(x => x.PostId)).ShouldEqual(numToCreate - 1);       //check numbers are consecutive
            }
        }

        [Test]
        public void Check02FillComputedNPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                const int numToCreate = 20;
                var snap = new DbSnapShot();        //empty

                //ATTEMPT
                db.FillComputedNPost(numToCreate);

                //VERIFY
                snap.CheckSnapShot(db, numToCreate, numToCreate, 2, 2);
                (db.Posts.Max(x => x.PostId) - db.Posts.Min(x => x.PostId)).ShouldEqual(numToCreate - 1);       //check numbers are consecutive
            }
        }

        [Test]
        public void Check05ListEfDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);

                //ATTEMPT
                db.ListEfDirect<Post>(0);

                //VERIFY
            }
        }

        [Test]
        public void Check06ListEfViaDtoOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);

                //ATTEMPT
                db.ListPostEfViaDto(0);

                //VERIFY
            }
        }

        [Test]
        public void Check10CreateEfDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);

                //ATTEMPT
                db.CreatePostEfDirect(0);

                //VERIFY
                db.Posts.Count().ShouldEqual(snap.NumPosts + 1);
            }
        }


        [Test]
        public void Check11UpdateEfDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var id = db.Posts.First().PostId;

                //ATTEMPT
                db.UpdatePostEfDirect(id);

                //VERIFY
                db.Posts.Count().ShouldEqual(snap.NumPosts);
            }
        }

        [Test]
        public void Check12DeleteEfDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var id = db.Posts.AsNoTracking().First().PostId;

                //ATTEMPT
                db.DeletePostEfDirect(id);

                //VERIFY
                db.Posts.Count().ShouldEqual(snap.NumPosts - 1);
            }
        }

        //---------------------------------------
        //generics

        [Test]
        public void Check15ListGenericDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);

                //ATTEMPT
                db.ListGenericDirect<Post>(0);

                //VERIFY
            }
        }

        [Test]
        public void Check16ListGenericViaDtoOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);

                //ATTEMPT
                db.ListPostGenericViaDto(0);

                //VERIFY
            }
        }

        [Test]
        public void Check17CreateGenericDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);

                //ATTEMPT
                db.CreatePostGenericDirect(0);

                //VERIFY
                db.Posts.Count().ShouldEqual(snap.NumPosts + 1);
            }
        }

        [Test]
        public void Check18UpdateGenericDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var id = db.Posts.AsNoTracking().First().PostId;

                //ATTEMPT
                db.UpdatePostGenericDirect(id);

                //VERIFY
                db.Posts.Count().ShouldEqual(snap.NumPosts);
            }
        }


        [Test]
        public void Check18UpdateGenericViaDtoOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var id = db.Posts.First().PostId;

                //ATTEMPT
                db.UpdatePostGenericViaDto(id);

                //VERIFY
                db.Posts.Count().ShouldEqual(snap.NumPosts);
            }
        }

        [Test]
        public void Check19DeleteGenericDirectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var id = db.Posts.AsNoTracking().First().PostId;

                //ATTEMPT
                db.DeletePostGenericDirect(id);

                //VERIFY
                db.Posts.Count().ShouldEqual(snap.NumPosts - 1);
            }
        }



    }
}
