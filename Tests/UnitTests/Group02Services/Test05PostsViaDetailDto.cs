using System;
using System.Data.Entity;
using System.Linq;
using GenericServices;
using GenericServices.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group02Services
{
    class Test05PostsViaDetailDto
    {

        [SetUp]
        public void SetUp()
        {
            using (var db = new TemplateWebAppDb())
            {
                DataLayerInitialise.InitialiseThis();
                DataLayerInitialise.ResetDatabaseToTestData(db);
            }
        }


        [Test]
        public void Check01ReferencesOk()
        {

            //SETUP    

            //ATTEMPT
            ICreateService<Post, DetailPostDto> createService = new CreateService<Post, DetailPostDto>(null);
            IDetailService<Post, DetailPostDto> detailService = new DetailService<Post, DetailPostDto>(null, null);
            IListService<Post, DetailPostDto> listService = new ListService<Post, DetailPostDto>(null);
            IUpdateService<Post, DetailPostDto> updateService = new UpdateService<Post, DetailPostDto>(null);

            //VERIFY
            (listService is ListService<Post, DetailPostDto>).ShouldEqual(true);
        }

        [Test]
        public void Check02ListPostOk()
        {
            using (var db = new TemplateWebAppDb())
            {
                //SETUP
                var service = new ListService<Post, DetailPostDto>(db);

                //ATTEMPT
                var query = service.GetList();
                var list = query.ToList();

                //VERIFY
                list.Count.ShouldEqual(3);
                var firstPost = db.Posts.Include(x => x.AllocatedTags).AsNoTracking().First();
                list[0].Title.ShouldEqual(firstPost.Title);
                list[0].Content.ShouldEqual(firstPost.Content);
                list[0].BloggerName.ShouldEqual(firstPost.Blogger.Name);
                list[0].TagNames.ShouldEqual("Good post, Ugly post");
                list[0].LastUpdatedUtc.Kind.ShouldEqual(DateTimeKind.Utc);
            }
        }

        [Test]
        public void Check03DetailPostOk()
        {
            using (var db = new TemplateWebAppDb())
            {
                //SETUP
                var service = new DetailService<Post, DetailPostDto>(db, new DetailPostDto());
                var firstPost = db.Posts.Include( x => x.AllocatedTags).AsNoTracking().First();

                //ATTEMPT
                var dto = service.GetDetail(x => x.PostId == firstPost.PostId);

                //VERIFY
                dto.PostId.ShouldEqual(firstPost.PostId);
                dto.BlogId.ShouldEqual(firstPost.BlogId);
                dto.BloggerName.ShouldEqual(firstPost.Blogger.Name);
                dto.Title.ShouldEqual(firstPost.Title);
                dto.Content.ShouldEqual(firstPost.Content);
                CollectionAssert.AreEqual(firstPost.AllocatedTags.Select(x => x.TagId), dto.AllocatedTags.Select(x => x.TagId));
            }
        }

        [Test]
        public void Check10CreateSetupServiceOk()
        {

            using (var db = new TemplateWebAppDb())
            {
                //SETUP
                var service = new CreateSetupService<Post, DetailPostDto>(db, new DetailPostDto());

                //ATTEMPT
                var dto = service.GetDto();

                //VERIFY
                dto.Bloggers.KeyValueList.Count.ShouldEqual( db.Blogs.Count()+1);
                dto.UserChosenTags.AllPossibleOptions.Count.ShouldEqual( db.Tags.Count());
            }
        }


        [Test]
        public void Check11CreatePostOk()
        {

            using (var db = new TemplateWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var service = new CreateService<Post, DetailPostDto>(db);
                var setupService = new CreateSetupService<Post, DetailPostDto>(db, new DetailPostDto());

                //ATTEMPT
                var dto = setupService.GetDto();
                dto.Title = Guid.NewGuid().ToString();
                dto.Content = "something to fill it as can't be empty";
                dto.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                dto.UserChosenTags.FinalSelection = db.Tags.Take(2).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = service.Create(dto);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, 1, 2);
                var post = db.Posts.Include(x => x.AllocatedTags).OrderByDescending( x => x.PostId).First();
                post.Title.ShouldEqual(dto.Title);
                post.BlogId.ShouldEqual(db.Blogs.First().BlogId);
                CollectionAssert.AreEqual(db.Tags.Take(2).Select(x => x.TagId), post.AllocatedTags.Select(x => x.TagId));
            }
        }

        [Test]
        public void Check12CreateFailRunsSetupSecondaryDataAgainOk()
        {

            using (var db = new TemplateWebAppDb())
            {
                //SETUP
                var service = new CreateService<Post, DetailPostDto>(db);
                var setupService = new CreateSetupService<Post, DetailPostDto>(db, new DetailPostDto());

                //ATTEMPT
                var dto = setupService.GetDto();
                dto.Title = Guid.NewGuid().ToString();
                dto.Content = null;                                 //this will fail 
                dto.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                dto.UserChosenTags.FinalSelection = db.Tags.Take(2).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = service.Create(dto);

                //VERIFY
                status.IsValid.ShouldEqual(false);
                dto.Bloggers.KeyValueList.Count.ShouldEqual(db.Blogs.Count() + 1);
                dto.UserChosenTags.AllPossibleOptions.Count.ShouldEqual(db.Tags.Count());
            }
        }

        [Test]
        public void Check15DtoCopyPropertiesOk()
        {
            using (var db = new TemplateWebAppDb())
            {
                //SETUP
                var firstPost = db.Posts.First();
                var setupService = new CreateSetupService<Post, DetailPostDto>(db, new DetailPostDto());
                var dto = setupService.GetDto();

                dto.PostId = firstPost.PostId;
                dto.BloggerName = "Should copy this blogger name";
                dto.BlogId = 333;
                dto.Title = "Should copy this title";
                dto.Content = "Should copy this content";
                dto.AllocatedTags = firstPost.AllocatedTags;
                dto.LastUpdated = new DateTime(2000, 1, 1);  

                //ATTEMPT
                var newData = new Post()
                {
                    Blogger = new Blog { Name = "Original Blog Name" },
                    BlogId = 777,
                    Content = "Original Content"
                };

                dto.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                dto.UserChosenTags.FinalSelection =
                    db.Tags.Take(2).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = dto.CopyDtoToData(db, dto, newData);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                newData.PostId.ShouldEqual(firstPost.PostId);
                newData.Title.ShouldEqual("Should copy this title");

                newData.BlogId.ShouldEqual(db.Blogs.First().BlogId);
                newData.Content.ShouldEqual("Should copy this content");
                //Can't check tags as that is written to database
            }
        }


        [Test]
        public void Check16UpdateSetupServiceOk()
        {

            using (var db = new TemplateWebAppDb())
            {
                //SETUP
                var setupService = new UpdateSetupService<Post, DetailPostDto>(db, new DetailPostDto());
                var firstPost = db.Posts.First();

                //ATTEMPT
                var dto = setupService.GetOriginal(x => x.PostId == firstPost.PostId);

                //VERIFY
                dto.Bloggers.KeyValueList.Count.ShouldEqual(db.Blogs.Count() + 1);
                dto.UserChosenTags.AllPossibleOptions.Count.ShouldEqual(db.Tags.Count());
            }
        }

        [Test]
        public void Check20UpdatePostLeaveTagSameOk()
        {

            using (var db = new TemplateWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var setupService = new UpdateSetupService<Post, DetailPostDto>(db, new DetailPostDto());
                var updateService = new UpdateService<Post, DetailPostDto>(db);
                var firstPost = db.Posts.First();

                //ATTEMPT
                var dto = setupService.GetOriginal(x => x.PostId == firstPost.PostId);
                dto.Title = Guid.NewGuid().ToString();
                dto.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                dto.UserChosenTags.FinalSelection = db.Tags.Take(2).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = updateService.Update(dto);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db);
                var post = db.Posts.Include(x => x.AllocatedTags).Single(x => x.PostId == firstPost.PostId);
                post.Title.ShouldEqual(dto.Title);
                post.BlogId.ShouldEqual(db.Blogs.First().BlogId);
                CollectionAssert.AreEqual(db.Tags.Take(2).Select(x => x.TagId), post.AllocatedTags.Select(x => x.TagId));
            }
        }


        [Test]
        public void Check21UpdatePostRemoveTagOk()
        {

            using (var db = new TemplateWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var setupService = new UpdateSetupService<Post, DetailPostDto>(db, new DetailPostDto());
                var updateService = new UpdateService<Post, DetailPostDto>(db);
                var firstPost = db.Posts.First();

                //ATTEMPT
                var dto = setupService.GetOriginal(x => x.PostId == firstPost.PostId);
                dto.Title = Guid.NewGuid().ToString();
                dto.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                dto.UserChosenTags.FinalSelection = db.Tags.Take(1).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = updateService.Update(dto);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, 0, -1);
                var post = db.Posts.Include( x=> x.AllocatedTags).Single(x => x.PostId == firstPost.PostId);
                post.Title.ShouldEqual(dto.Title);
                post.BlogId.ShouldEqual(db.Blogs.First().BlogId);
                CollectionAssert.AreEqual(db.Tags.Take(1).Select(x => x.TagId), post.AllocatedTags.Select(x => x.TagId));
            }
        }

        [Test]
        public void Check22UpdatePostAddTagOk()
        {

            using (var db = new TemplateWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var setupService = new UpdateSetupService<Post, DetailPostDto>(db, new DetailPostDto());
                var updateService = new UpdateService<Post, DetailPostDto>(db);
                var firstPost = db.Posts.First();

                //ATTEMPT
                var dto = setupService.GetOriginal(x => x.PostId == firstPost.PostId);
                dto.Title = Guid.NewGuid().ToString();
                dto.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                dto.UserChosenTags.FinalSelection = db.Tags.Take(3).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = updateService.Update(dto);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, 0, 1);
                var post = db.Posts.Include(x => x.AllocatedTags).Single(x => x.PostId == firstPost.PostId);
                post.Title.ShouldEqual(dto.Title);
                post.BlogId.ShouldEqual(db.Blogs.First().BlogId);
                CollectionAssert.AreEquivalent(db.Tags.Take(3).Select(x => x.TagId), post.AllocatedTags.Select(x => x.TagId));
            }
        }

        [Test]
        public void Check25UpdatePostFailRunsSetupSecondaryDataAgainOk()
        {

            using (var db = new TemplateWebAppDb())
            {
                //SETUP
                var setupService = new UpdateSetupService<Post, DetailPostDto>(db, new DetailPostDto());
                var updateService = new UpdateService<Post, DetailPostDto>(db);
                var firstPost = db.Posts.First();

                //ATTEMPT
                var dto = setupService.GetOriginal(x => x.PostId == firstPost.PostId);
                dto.Title = null;                   //that will fail
                dto.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                dto.UserChosenTags.FinalSelection = db.Tags.Take(3).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = updateService.Update(dto);

                //VERIFY
                status.IsValid.ShouldEqual(false);
                dto.Bloggers.KeyValueList.Count.ShouldEqual(db.Blogs.Count() + 1);
                dto.UserChosenTags.AllPossibleOptions.Count.ShouldEqual(db.Tags.Count());
            }
        }

    }
}
