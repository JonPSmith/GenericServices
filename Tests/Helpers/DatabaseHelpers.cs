using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;

namespace Tests.Helpers
{
    static class DatabaseHelpers
    {

        public static IQueryable<T> GetListEfDirect<T>(this SampleWebAppDb db) where T : class
        {
            return db.Set<T>().AsNoTracking();
        }

        public static IQueryable<SimplePostDto> GetListPostEfViaDto(this SampleWebAppDb db)
        {
            return db.Set<Post>().AsNoTracking().Select(x => new SimplePostDto
            {
                PostId = x.PostId,
                BloggerName = x.Blogger.Name,
                Title = x.Title,
                Tags = x.Tags,
                LastUpdated = x.LastUpdated
            });
        }

        public static void CreatePostEfDirect(this SampleWebAppDb db)
        {
            var guidString = Guid.NewGuid().ToString("N");
            var postClass = new Post
            {
                Title = guidString,
                Content = guidString,
                Blogger = db.Blogs.First(),
                Tags = new Collection<Tag> {  db.Tags.First()}
            };
            db.Posts.Add(postClass);
            var status = db.SaveChangesWithValidation();
            status.IsValid.ShouldEqual(true, status.Errors);
        }

        public static void UpdatePostEfDirect(this SampleWebAppDb db, int postId)
        {

            var postClass = db.Posts.Include( x => x.Blogger).Include( x => x.Tags).Single( x => x.PostId == postId);

            var guidString = Guid.NewGuid().ToString("N");
            postClass.Title = guidString;
            postClass.Content = guidString;
            postClass.Blogger = db.Blogs.First();
            postClass.Tags = new Collection<Tag> {db.Tags.First()};

            var status = db.SaveChangesWithValidation();
            status.IsValid.ShouldEqual(true, status.Errors);
        }

        public static void DeletePostEfDirect(this SampleWebAppDb db, int postId)
        {
            var postClass = new Post
            {
                PostId = postId
            };
            db.Entry(postClass).State = EntityState.Deleted;
            var status = db.SaveChangesWithValidation();
            status.IsValid.ShouldEqual(true, status.Errors);
        }

        public static void FillComputed(this SampleWebAppDb db, int totalOfEach)
        {
            //clear the current
            db.Posts.RemoveRange(db.Posts);
            db.Tags.RemoveRange(db.Tags);
            db.Blogs.RemoveRange(db.Blogs);
            db.PostTagGrades.RemoveRange(db.PostTagGrades);
            db.SaveChanges();

            var tags = BuildTags(totalOfEach);
            db.Tags.AddRange(tags);
            var bloggers = BuildBloggers(totalOfEach);
            db.Blogs.AddRange(bloggers);
            var posts = BuildPosts(tags, bloggers, totalOfEach);
            db.Posts.AddRange(posts);
            var grades = BuildGrades(tags, posts, totalOfEach);
            db.PostTagGrades.AddRange(grades);
            var status = db.SaveChangesWithValidation();
            status.IsValid.ShouldEqual(true, status.Errors);

        }


        private static List<Tag> BuildTags(int totalOfEach)
        {
            var result = new List<Tag>();
            for (int i = 0; i < totalOfEach; i++)
            {
                var guidString = Guid.NewGuid().ToString("N");
                result.Add(new Tag
                {
                    Name = guidString,
                    Slug = guidString
                });
            }
            return result;
        }

        private static List<Blog> BuildBloggers(int totalOfEach)
        {
            var result = new List<Blog>();
            for (int i = 0; i < totalOfEach; i++)
            {
                var guidString = Guid.NewGuid().ToString("N");
                result.Add(new Blog
                {
                    Name = guidString,
                    EmailAddress = guidString
                });
            }
            return result;
        }

        private static List<Post> BuildPosts(List<Tag> tags, List<Blog> bloggers, int totalOfEach)
        {
            var result = new List<Post>();
            for (int i = 0; i < totalOfEach; i++)
            {
                var guidString = Guid.NewGuid().ToString("N");
                result.Add(new Post
                {
                    Title = guidString,
                    Content = guidString,
                    Blogger = bloggers[i],
                    Tags = new Collection<Tag> { tags[i / 2], tags[(totalOfEach - 1 + i) / 2] }
                });
            }
            return result;
        }

        private static List<PostTagGrade> BuildGrades(List<Tag> tags, List<Post> posts, int totalOfEach)
        {
            var result = new List<PostTagGrade>();
            for (int i = 0; i < totalOfEach; i++)
            {
                result.Add(new PostTagGrade
                {
                    PostPart = posts[i],
                    TagPart = tags[i],
                    Grade = 1
                });
            }
            return result;
        }

    }
}
