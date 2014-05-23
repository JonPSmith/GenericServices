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
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<SampleWebAppDb>());
        }

        public static void ResetDatabaseToTestData(SampleWebAppDb context)
        {

            if (context.Blogs.Any())
            {
                context.Posts.ToList().ForEach(x => context.Posts.Remove(x));
                context.Tags.ToList().ForEach(x => context.Tags.Remove(x));
                context.Blogs.ToList().ForEach(x => context.Blogs.Remove(x));
                context.SaveChanges();
            }

            var goodTag = new Tag { Name = "Good post", Slug = "good" };
            var badTag = new Tag { Name = "Bad post", Slug = "bad" };
            var uglyTag = new Tag { Name = "Ugly post", Slug = "ugly" };

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
                Content = "A fine set of words.\nIn two lines.",
                Tags = new List<Tag> { goodTag, uglyTag }
            };

            var jonPost2 = new Post
            {
                Blogger = jonBlogger,
                Title = "Second post, which isn't bad",
                Content = "Another fine set of words.\nWith this line\nand another line, making three lines.",
                Tags = new List<Tag> { badTag }
            };

            var fredPost = new Post
            {
                Blogger = fredBlogger,
                Title = "Freds good post",
                Content = "He hasn't got much to say.",
                Tags = new List<Tag> { uglyTag, badTag }
            };

            jonBlogger.Posts = new List<Post> { jonPost1, jonPost2 };
            fredBlogger.Posts = new List<Post> { fredPost };

            context.Blogs.Add(jonBlogger);
            context.Blogs.Add(fredBlogger);
            context.SaveChanges();
        }
    }

}
