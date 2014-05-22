using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group01DataClasses
{
    class Tests02Validation
    {

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            using (var db = new SampleWebAppDb())
            {
                DataLayerInitialise.InitialiseThis();
                DataLayerInitialise.ResetDatabaseToTestData(db);
            }
        }

        [Test]
        public void Check01ValidateTagOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);

                //ATTEMPT
                var dupTag = new Tag { Name = "non-duplicate slug", Slug = Guid.NewGuid().ToString("N") };
                db.Tags.Add(dupTag);
                var status = db.SaveChangesWithValidation();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, 0,0,0,1);
            }
        }

        [Test]
        public void Check02ValidateTagError()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var existingTag = db.Tags.First();

                //ATTEMPT
                var dupTag = new Tag {Name = "duplicate slug", Slug = existingTag.Slug};
                db.Tags.Add(dupTag);
                var status = db.SaveChangesWithValidation();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("The Slug on tag 'duplicate slug' must be unique.");
            }
        }

        [Test]
        public void Check10ValidatePostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var existingTag = db.Tags.First();
                var existingBlogger = db.Blogs.First();

                //ATTEMPT
                var newPost = new Post()
                {
                    Blogger = existingBlogger,
                    Title = "Test post",
                    Content = "Nothing special",
                    Tags = new[] { existingTag }
                };
                db.Posts.Add(newPost);
                var status = db.SaveChangesWithValidation();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db,1,1);
            }
        }


        [Test]
        public void Check15ValidatePostTitleError()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var existingTag = db.Tags.First();
                var existingBlogger = db.Blogs.First();

                //ATTEMPT
                var newPost = new Post()
                {
                    Blogger = existingBlogger,
                    Title = "Test post!",
                    Content = "Nothing special",
                    Tags = new[] { existingTag }
                };
                db.Posts.Add(newPost);
                var status = db.SaveChangesWithValidation();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("Sorry, but you can't get too excited and include a ! in the title.");
            }
        }

        [Test]
        public void Check16ValidatePostTitleError()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var existingTag = db.Tags.First();
                var existingBlogger = db.Blogs.First();

                //ATTEMPT
                var newPost = new Post()
                {
                    Blogger = existingBlogger,
                    Title = "Test post?",
                    Content = "Nothing special",
                    Tags = new[] { existingTag }
                };
                db.Posts.Add(newPost);
                var status = db.SaveChangesWithValidation();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("Sorry, but you can't ask a question, i.e. the title can't end with '?'.");
            }
        }

        [Test]
        public void Check20ValidatePostContentOneError()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var existingTag = db.Tags.First();
                var existingBlogger = db.Blogs.First();

                //ATTEMPT
                var newPost = new Post()
                {
                    Blogger = existingBlogger,
                    Title = "Test post",
                    Content = "Should not end sentence with sheep.",
                    Tags = new[] { existingTag }
                };
                db.Posts.Add(newPost);
                var status = db.SaveChangesWithValidation();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("Sorry. Not allowed to end a sentance with 'sheep'.");
            }
        }


        [Test]
        public void Check21ValidatePostContentTwoErrors()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var existingTag = db.Tags.First();
                var existingBlogger = db.Blogs.First();

                //ATTEMPT
                var newPost = new Post()
                {
                    Blogger = existingBlogger,
                    Title = "Test post",
                    Content = "Should not end sentence with sheep. Nor end sentence with lamb.",
                    Tags = new[] { existingTag }
                };
                db.Posts.Add(newPost);
                var status = db.SaveChangesWithValidation();

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(2);
                status.Errors[0].ErrorMessage.ShouldEqual("Sorry. Not allowed to end a sentance with 'sheep'.");
                status.Errors[1].ErrorMessage.ShouldEqual("Sorry. Not allowed to end a sentance with 'lamb'.");
            }
        }
    }
}

