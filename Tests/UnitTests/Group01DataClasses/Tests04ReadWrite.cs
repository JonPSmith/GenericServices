#region licence
// The MIT License (MIT)
// 
// Filename: Tests04ReadWrite.cs
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
using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using GenericServices;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group01DataClasses
{
    class Tests04ReadWrite
    {

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
        public void Check01ReadBlogsNoPostsOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP

                //ATTEMPT
                var blogs = db.Blogs.ToList();

                //VERIFY
                blogs.Count.ShouldEqual(2);
                blogs.All( x => x.Posts == null).ShouldEqual(true);
            }
        }

        public void Check01ReadBlogsWithPostsOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP

                //ATTEMPT
                var blogs = db.Blogs.Include( x => x.Posts).ToList();

                //VERIFY
                blogs.Count.ShouldEqual(2);
                blogs.All(x => x.Posts != null).ShouldEqual(true);
                blogs.All(x => x.Posts.All(y => y.Tags == null)).ShouldEqual(true);
            }
        }

        [Test]
        public void Check02ReadBlogsWithPostTagsOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP

                //ATTEMPT
                var blogs = db.Blogs.Include(x => x.Posts.Select(y => y.Tags)).ToList();

                //VERIFY
                blogs.Count.ShouldEqual(2);
                blogs.All(x => x.Posts != null).ShouldEqual(true);
                blogs.All(x => x.Posts.All(y => y.Tags != null)).ShouldEqual(true);

            }
        }

        [Test]
        public void Check05ReadPostsOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP

                //ATTEMPT
                var posts = db.Posts.ToList();

                //VERIFY
                posts.Count.ShouldEqual(3);
                posts.All(x => x.Blogger != null).ShouldEqual(true);
                posts.All(x => x.Tags == null).ShouldEqual(true);
            }
        }


        [Test]
        public void Check06ReadPostsWithTagsOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP

                //ATTEMPT
                var posts = db.Posts.Include( x => x.Tags).ToList();

                //VERIFY
                posts.Count.ShouldEqual(3);
                posts.All(x => x.Blogger != null).ShouldEqual(true);
                posts.All(x => x.Tags != null).ShouldEqual(true);
            }
        }

        [Test]
        public void Check10ReadTAllocatedTagsWithUglySlugOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP

                //ATTEMPT
                var postsWithuglyTags = db.Posts.Where(x => x.Tags.Any( y => y.Slug == "ugly")).ToList();
                var uglyTagPosts = db.Tags.Include(x => x.Posts).Single(y => y.Slug == "ugly").Posts;

                //VERIFY
                postsWithuglyTags.Count.ShouldEqual(2);
                uglyTagPosts.Count().ShouldEqual(2);
            }
        }

        //-----------------------------------------------------------------
        //now adding new posts, tags etc.

        [Test]
        public void Check20AddPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);

                //ATTEMPT
                var uglyTag = db.Tags.Single(x => x.Slug == "ugly");
                var jonBlogger = db.Blogs.First();
                var newPost = new Post
                {
                    Blogger = jonBlogger,
                    Content = "a few simple words.",
                    Title = "A new post",
                    Tags = new[] { uglyTag }
                };

                db.Posts.Add(newPost);
                var status = db.SaveChangesWithChecking();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, 1, 1);
                var uglyPosts = db.Tags.Include(x => x.Posts).Single(y => y.Slug == "ugly").Posts;
                uglyPosts.Count.ShouldEqual(3);
            }
        }

        [Test]
        public void Check21CheckUpdateSimpleOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var newGuid = Guid.NewGuid().ToString();

                //ATTEMPT
                var firstPost = db.Posts.First();
                firstPost.Title = newGuid;
                var status = db.SaveChangesWithChecking();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db);
                db.Posts.First().Title.ShouldEqual(newGuid);
            }
        }


        [Test]
        public void Check22CheckUpdateLastUpdatedOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var firstPost = db.Posts.First();
                var originalDateTime = firstPost.LastUpdated;
                Thread.Sleep(400);

                //ATTEMPT
                firstPost.Title = Guid.NewGuid().ToString();
                var status = db.SaveChangesWithChecking();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db);
                Assert.GreaterOrEqual(db.Posts.First().LastUpdated.Subtract(originalDateTime).Milliseconds, 400);
            }
        }

        [Test]
        public void Check25UpdatePostToAddTagOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var firstPost = db.Posts.First();
                var tagsNotInFirstPostTracked = db.Tags.Where(x => x.Posts.All(y => y.PostId != firstPost.PostId)).ToList();

                //ATTEMPT
                db.Entry(firstPost).Collection( x => x.Tags).Load();
                tagsNotInFirstPostTracked.ForEach( x => firstPost.Tags.Add( x));
                var status = db.SaveChangesWithChecking();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, 0, 1);
                firstPost = db.Posts.Include(x => x.Tags).First();
                firstPost.Tags.Count.ShouldEqual(3);
            }
        }

        [Test]
        public void Check26ReplaceTagsOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var firstPost = db.Posts.First();
                var tagsNotInFirstPostTracked = db.Tags.Where(x => x.Posts.All(y => y.PostId != firstPost.PostId)).ToList();

                //ATTEMPT
                db.Entry(firstPost).Collection(x => x.Tags).Load();
                firstPost.Tags = tagsNotInFirstPostTracked;
                var status = db.SaveChangesWithChecking();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, 0, -1);
                firstPost = db.Posts.Include(x => x.Tags).First();
                firstPost.Tags.Count.ShouldEqual(1);
            }
        }

        [Test]
        public void Check27DeleteConnectedPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var lastPost = db.Posts.Include(x => x.Tags).ToList().Last();
                var numTags = lastPost.Tags.Count;

                //ATTEMPT
                db.Posts.Remove(lastPost);
                var status = db.SaveChangesWithChecking();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, -1, -numTags);
            }
        }

        [Test]
        public void Check28DeleteViaAttachOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var lastPost = db.Posts.Include(x => x.Tags).AsNoTracking().ToList().Last();
                var numTags = lastPost.Tags.Count;

                //ATTEMPT
                var postToDelete = new Post {PostId = lastPost.PostId};
                db.Posts.Attach(postToDelete);
                db.Posts.Remove(postToDelete);
                var status = db.SaveChangesWithChecking();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, -1, -numTags);
            }
        }


        //---------------------------------------------------------
        //now deleting the PostTagGrade

        [Test]
        public void Check30FindMultipleKeyOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var firstPostTagGrades = db.PostTagGrades.First();

                //ATTEMPT
                var foundPtg = db.PostTagGrades.Find(firstPostTagGrades.PostId, firstPostTagGrades.TagId);

                //VERIFY
                foundPtg.ShouldNotEqualNull();
                foundPtg.Grade.ShouldEqual(5);
            }
        }

        [Test]
        public void Check31DeleteViaAttachOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var firstPostTagGrades = db.PostTagGrades.AsNoTracking().First();

                //ATTEMPT
                var ptgToDelete = new PostTagGrade
                {
                    PostId = firstPostTagGrades.PostId,
                    TagId = firstPostTagGrades.TagId
                };
                db.PostTagGrades.Attach(ptgToDelete);
                ((IObjectContextAdapter)db).ObjectContext.DeleteObject(ptgToDelete);
                var status = db.SaveChangesWithChecking();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, 0,0,0,0,-1);
            }
        }

        [Test]
        public void Check32DeleteViaRemoveOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var firstPostTagGrades = db.PostTagGrades.AsNoTracking().First();

                //ATTEMPT
                var ptgToDelete = new PostTagGrade
                {
                    PostId = firstPostTagGrades.PostId,
                    TagId = firstPostTagGrades.TagId
                };
                db.PostTagGrades.Attach(ptgToDelete);
                db.PostTagGrades.Remove(ptgToDelete);
                var status = db.SaveChangesWithChecking();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, 0, 0, 0, 0, -1);
            }
        }

    }
}
