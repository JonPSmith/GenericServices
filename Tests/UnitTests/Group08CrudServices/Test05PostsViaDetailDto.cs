using System;
using System.Data.Entity;
using System.Linq;
using GenericServices;
using GenericServices.Core;
using GenericServices.Services;
using GenericServices.Services.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group08CrudServices
{
    class Test05PostsViaDetailDto
    {

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            new DetailPostDto().CacheSetup();
        }

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
        public void Check01ReferencesOk()
        {

            //SETUP    

            //ATTEMPT
            ICreateService<Post, DetailPostDto> createService = new CreateService<Post, DetailPostDto>(null);
            IDetailService<Post, DetailPostDto> detailService = new DetailService<Post, DetailPostDto>(null);
            IListService<Post, DetailPostDto> listService = new ListService<Post, DetailPostDto>(null);
            IUpdateService<Post, DetailPostDto> updateService = new UpdateService<Post, DetailPostDto>(null);

            //VERIFY
            (listService is ListService<Post, DetailPostDto>).ShouldEqual(true);
        }

        [Test]
        public void Check02ListPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new ListService<Post, DetailPostDto>(db);

                //ATTEMPT
                var status = service.GetMany().TryManyWithPermissionChecking();

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.Count().ShouldEqual(3);
                var firstPost = db.Posts.Include(x => x.Tags).ToList().First(x => x.PostId == status.Result.First().PostId);
                status.Result.First().Title.ShouldEqual(firstPost.Title);
                status.Result.First().Content.ShouldEqual(firstPost.Content);
                status.Result.First().BloggerName.ShouldEqual(firstPost.Blogger.Name);
                status.Result.First().TagNames.ShouldEqual("Ugly post, Good post");
                status.Result.First().LastUpdatedUtc.Kind.ShouldEqual(DateTimeKind.Utc);
            }
        }

        [Test]
        public void Check03DetailPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailService<Post, DetailPostDto>(db);
                var firstPost = db.Posts.Include( x => x.Tags).AsNoTracking().First();

                //ATTEMPT
                var status = service.GetDetailUsingWhere(x => x.PostId == firstPost.PostId);

                //VERIFY
                status.IsValid.ShouldEqual(true);
                status.Result.PostId.ShouldEqual(firstPost.PostId);
                status.Result.BlogId.ShouldEqual(firstPost.BlogId);
                status.Result.BloggerName.ShouldEqual(firstPost.Blogger.Name);
                status.Result.Title.ShouldEqual(firstPost.Title);
                status.Result.Content.ShouldEqual(firstPost.Content);
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.TagId), status.Result.Tags.Select(x => x.TagId));
            }
        }

        [Test]
        public void Check10CreateSetupServiceOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateSetupService<Post, DetailPostDto>(db);

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

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var service = new CreateService<Post, DetailPostDto>(db);
                var setupService = new CreateSetupService<Post, DetailPostDto>(db);

                //ATTEMPT
                var dto = setupService.GetDto();
                dto.Title = Guid.NewGuid().ToString();
                dto.Content = "something to fill it as can't be empty";
                dto.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                dto.UserChosenTags.FinalSelection = db.Tags.OrderBy( x => x.TagId).Take(2).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = service.Create(dto);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, 1, 2);
                var post = db.Posts.Include(x => x.Tags).OrderByDescending( x => x.PostId).First();
                post.Title.ShouldEqual(dto.Title);
                post.BlogId.ShouldEqual(db.Blogs.First().BlogId);
                CollectionAssert.AreEqual(db.Tags.OrderBy(x => x.TagId).Take(2).Select(x => x.TagId), post.Tags.Select(x => x.TagId));
            }
        }

        [Test]
        public void Check12CreateFailRunsSetupSecondaryDataAgainOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateService<Post, DetailPostDto>(db);
                var setupService = new CreateSetupService<Post, DetailPostDto>(db);

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
        public void Check13CreateServiceResetWorksOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateService<Post, DetailPostDto>(db);

                //ATTEMPT
                var dto = new DetailPostDto();
                service.ResetDto(dto);

                //VERIFY
                dto.Bloggers.KeyValueList.Count.ShouldEqual(db.Blogs.Count() + 1);
                dto.UserChosenTags.AllPossibleOptions.Count.ShouldEqual(db.Tags.Count());
            }
        }

        //[Test]
        //public void Check15DtoCopyPropertiesOk()
        //{
        //    using (var db = new SampleWebAppDb())
        //    {
        //        //SETUP
        //        var firstPost = db.Posts.First();
        //        var setupService = new CreateSetupService<Post, DetailPostDto>(db);
        //        var dto = setupService.GetDto();

        //        dto.PostId = firstPost.PostId;
        //        dto.BloggerName = "Should copy this blogger name";
        //        dto.BlogId = 333;
        //        dto.Title = "Should copy this title";
        //        dto.Content = "Should copy this content";
        //        dto.Tags = firstPost.Tags;
        //        dto.LastUpdated = new DateTime(2000, 1, 1);  

        //        //ATTEMPT
        //        var newData = new Post()
        //        {
        //            Blogger = new Blog { Name = "Original Blog Name" },
        //            BlogId = 777,
        //            Content = "Original Content"
        //        };

        //        dto.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
        //        dto.UserChosenTags.FinalSelection =
        //            db.Tags.Take(2).ToList().Select(x => x.TagId.ToString("D")).ToArray();
        //        var status = dto.CopyDtoToData(db, dto, newData);

        //        //VERIFY
        //        status.IsValid.ShouldEqual(true, status.Errors);
        //        newData.PostId.ShouldEqual(firstPost.PostId);
        //        newData.Title.ShouldEqual("Should copy this title");

        //        newData.BlogId.ShouldEqual(db.Blogs.First().BlogId);
        //        newData.Content.ShouldEqual("Should copy this content");
        //        //Can't check tags as that is written to database
        //    }
        //}


        [Test]
        public void Check16UpdateSetupServiceOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var setupService = new UpdateSetupService<Post, DetailPostDto>(db);
                var firstPost = db.Posts.First();

                //ATTEMPT
                var status = setupService.GetOriginal(firstPost.PostId);

                //VERIFY
                status.IsValid.ShouldEqual(true);
                status.Result.Bloggers.KeyValueList.Count.ShouldEqual(db.Blogs.Count() + 1);
                status.Result.UserChosenTags.AllPossibleOptions.Count.ShouldEqual(db.Tags.Count());
            }
        }

        [Test]
        public void Check20UpdatePostLeaveTagSameOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var setupService = new UpdateSetupService<Post, DetailPostDto>(db);
                var updateService = new UpdateService<Post, DetailPostDto>(db);
                var firstPost = db.Posts.Include(x => x.Tags).First();

                //ATTEMPT
                var setupStatus = setupService.GetOriginal(firstPost.PostId);
                setupStatus.IsValid.ShouldEqual(true);
                setupStatus.Result.Title = Guid.NewGuid().ToString();
                setupStatus.Result.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                setupStatus.Result.UserChosenTags.FinalSelection = firstPost.Tags.Select(x => x.TagId.ToString("D")).ToArray();
                var status = updateService.Update(setupStatus.Result);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db);
                var post = db.Posts.Include(x => x.Tags).Single(x => x.PostId == firstPost.PostId);
                post.Title.ShouldEqual(setupStatus.Result.Title);
                post.BlogId.ShouldEqual(db.Blogs.First().BlogId);
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.TagId), post.Tags.Select(x => x.TagId));
            }
        }


        [Test]
        public void Check21UpdatePostRemoveTagOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var setupService = new UpdateSetupService<Post, DetailPostDto>(db);
                var updateService = new UpdateService<Post, DetailPostDto>(db);
                var firstPost = db.Posts.First();

                //ATTEMPT
                var setupStatus = setupService.GetOriginal(firstPost.PostId);
                setupStatus.IsValid.ShouldEqual(true);
                setupStatus.Result.Title = Guid.NewGuid().ToString();
                setupStatus.Result.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                setupStatus.Result.UserChosenTags.FinalSelection = db.Tags.Take(1).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = updateService.Update(setupStatus.Result);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, 0, -1);
                var post = db.Posts.Include( x=> x.Tags).Single(x => x.PostId == firstPost.PostId);
                post.Title.ShouldEqual(setupStatus.Result.Title);
                post.BlogId.ShouldEqual(db.Blogs.First().BlogId);
                CollectionAssert.AreEqual(db.Tags.Take(1).Select(x => x.TagId), post.Tags.Select(x => x.TagId));
            }
        }

        [Test]
        public void Check22UpdatePostAddTagOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var setupService = new UpdateSetupService<Post, DetailPostDto>(db);
                var updateService = new UpdateService<Post, DetailPostDto>(db);
                var firstPost = db.Posts.First();

                //ATTEMPT
                var setupStatus = setupService.GetOriginal(firstPost.PostId);
                setupStatus.IsValid.ShouldEqual(true, setupStatus.Errors);
                setupStatus.Result.Title = Guid.NewGuid().ToString();
                setupStatus.Result.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                setupStatus.Result.UserChosenTags.FinalSelection = db.Tags.Take(3).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = updateService.Update(setupStatus.Result);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                snap.CheckSnapShot(db, 0, 1);
                var post = db.Posts.Include(x => x.Tags).Single(x => x.PostId == firstPost.PostId);
                post.Title.ShouldEqual(setupStatus.Result.Title);
                post.BlogId.ShouldEqual(db.Blogs.First().BlogId);
                CollectionAssert.AreEquivalent(db.Tags.Take(3).Select(x => x.TagId), post.Tags.Select(x => x.TagId));
            }
        }

        [Test]
        public void Check25UpdatePostFailRunsSetupSecondaryDataAgainOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var setupService = new UpdateSetupService<Post, DetailPostDto>(db);
                var updateService = new UpdateService<Post, DetailPostDto>(db);
                var firstPost = db.Posts.First();

                //ATTEMPT
                var setupStatus = setupService.GetOriginal(firstPost.PostId);
                setupStatus.IsValid.ShouldEqual(true, setupStatus.Errors);
                setupStatus.Result.Title = null;                   //that will fail
                setupStatus.Result.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                setupStatus.Result.UserChosenTags.FinalSelection = db.Tags.Take(3).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = updateService.Update(setupStatus.Result);

                //VERIFY
                status.IsValid.ShouldEqual(false);
                setupStatus.Result.Bloggers.KeyValueList.Count.ShouldEqual(db.Blogs.Count() + 1);
                setupStatus.Result.UserChosenTags.AllPossibleOptions.Count.ShouldEqual(db.Tags.Count());
            }
        }

        [Test]
        public void Check26UpdateServiceResetWorksOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateService<Post, DetailPostDto>(db);

                //ATTEMPT
                var dto = new DetailPostDto();
                service.ResetDto(dto);

                //VERIFY
                dto.Bloggers.KeyValueList.Count.ShouldEqual(db.Blogs.Count() + 1);
                dto.UserChosenTags.AllPossibleOptions.Count.ShouldEqual(db.Tags.Count());
            }
        }
    }
}
