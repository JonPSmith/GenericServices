using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
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
                DataLayerInitialise.ResetDatabaseToTestData(db);
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
                var status = db.SaveChangesWithValidation();

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
                var status = db.SaveChangesWithValidation();

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
                var status = db.SaveChangesWithValidation();

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
                var badTag = db.Tags.Single(x => x.Slug == "bad");
                var firstPost = db.Posts.First();

                //ATTEMPT
                db.Entry(firstPost).Collection( x => x.Tags).Load();
                firstPost.Tags.Add( badTag);
                var status = db.SaveChangesWithValidation();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, 0, 1);
                firstPost = db.Blogs.Include(x => x.Posts.Select(y => y.Tags)).First().Posts.First();
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
                var status = db.SaveChangesWithValidation();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, 0, -1);
                firstPost = db.Blogs.Include(x => x.Posts.Select(y => y.Tags)).First().Posts.First();
                firstPost.Tags.Count.ShouldEqual(1);
            }
        }

    }
}
