using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Tests.DataClasses.Concrete;

namespace Tests.DataClasses
{
    public static class DataLayerInitialise
    {

        /// <summary>
        /// This should be called at Startup
        /// </summary>
        public static void InitialiseThis()
        {
            //Initialise the database
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<TemplateWebAppDb>());
        }

        public static void ResetDatabaseToTestData(TemplateWebAppDb context)
        {

            if (context.Blogs.Any())
            {
                context.PostTagLinks.RemoveRange(context.PostTagLinks);
                context.Posts.RemoveRange(context.Posts);
                context.Tags.RemoveRange(context.Tags);
                context.Blogs.RemoveRange(context.Blogs);
                context.SaveChanges();
            }

            var goodTag = new Tag {Name = "Good post", Slug = "good"};
            var badTag = new Tag {Name = "Bad post", Slug = "bad"};
            var uglyTag = new Tag {Name = "Ugly post", Slug = "ugly"};

            var jonBlogger = new Blog
            {
                Name = "Jon Smith",
                EmailAddress = "jon.smith@nospam.com"
            };
            var fredBlogger = new Blog
            {
                Name = "Fred Bloggs",
                EmailAddress = "fred.blogs@nospam.com"
            };

            var jonPost1 = new Post
            {
                Blogger = jonBlogger,
                Title = "First great post",
                Content = "A fine set of words.\nIn two lines."
            };
            jonPost1.AllocatedTags = new List<PostTagLink>
            {
                new PostTagLink { InPost = jonPost1, HasTag = goodTag },
                new PostTagLink { InPost = jonPost1, HasTag = uglyTag}
            };

            var jonPost2 = new Post
            {
                Blogger = jonBlogger,
                Title = "Second post, which isn't bad",
                Content = "Another fine set of words.\nWith this line\nand another line, making three lines.",
            };
            jonPost2.AllocatedTags = new List<PostTagLink>
            {
                new PostTagLink {InPost = jonPost2, HasTag = badTag}
            };

            var fredPost = new Post
            {
                Blogger = fredBlogger,
                Title = "First great post",
                Content = "A fine set of words.\nIn two lines."
            };
            fredPost.AllocatedTags = new List<PostTagLink>
            {
                new PostTagLink { InPost = fredPost, HasTag = uglyTag },
                new PostTagLink { InPost = fredPost, HasTag = badTag}
            };

            jonBlogger.Posts = new List<Post> {jonPost1, jonPost2};
            fredBlogger.Posts = new List<Post>{ fredPost};

            context.Blogs.Add(jonBlogger);
            context.Blogs.Add(fredBlogger);
            context.SaveChanges();
        }
    }

}
