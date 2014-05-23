using System;
using System.Collections.Generic;
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
                blogs.All(x => x.Posts.All(y => y.AllocatedTags == null)).ShouldEqual(true);
            }
        }

        [Test]
        public void Check02ReadBlogsWithPostTagsOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP

                //ATTEMPT
                var blogs = db.Blogs.Include(x => x.Posts.Select(y => y.AllocatedTags)).ToList();

                //VERIFY
                blogs.Count.ShouldEqual(2);
                blogs.All(x => x.Posts != null).ShouldEqual(true);
                blogs.All(x => x.Posts.All(y => y.AllocatedTags != null)).ShouldEqual(true);

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
                posts.All(x => x.AllocatedTags == null).ShouldEqual(true);
            }
        }


        [Test]
        public void Check06ReadPostsWithTagsOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP

                //ATTEMPT
                var posts = db.Posts.Include( x => x.AllocatedTags).ToList();

                //VERIFY
                posts.Count.ShouldEqual(3);
                posts.All(x => x.Blogger != null).ShouldEqual(true);
                posts.All(x => x.AllocatedTags != null).ShouldEqual(true);
            }
        }

        [Test]
        public void Check10ReadTAllocatedTagsWithUglySlugOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP

                //ATTEMPT
                var uglyTags = db.PostTagLinks.Where(x => x.HasTag.Slug == "ugly").Select(x => x.InPost).ToList();

                //VERIFY
                uglyTags.Count.ShouldEqual(2);
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
                    Title = "A new post"
                };
                newPost.AllocatedTags = new List<PostTagLink>
                {
                    new PostTagLink {InPost = newPost, HasTag = uglyTag}
                };
                db.Posts.Add(newPost);
                var status = db.SaveChangesWithValidation();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, 1, 1);
                var uglyPosts =
                    db.PostTagLinks.Include(x => x.InPost)
                        .Where(x => x.HasTag.Slug == "ugly")
                        .Select(x => x.InPost)
                        .ToList();
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

                //ATTEMPT
                var badTag = db.Tags.Single(x => x.Slug == "bad");
                var jonBlogger = db.Blogs.Include( x => x.Posts).First();
                var firstPost = jonBlogger.Posts.First();
                db.Entry(firstPost).Collection( x => x.AllocatedTags).Load();
                firstPost.AllocatedTags.Add( new PostTagLink { InPost = firstPost, HasTag = badTag});
                var status = db.SaveChangesWithValidation();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, 0, 1);
                firstPost = db.Blogs.Include(x => x.Posts.Select(y => y.AllocatedTags)).First().Posts.First();
                firstPost.AllocatedTags.Count.ShouldEqual(3);
            }
        }

    }
}
