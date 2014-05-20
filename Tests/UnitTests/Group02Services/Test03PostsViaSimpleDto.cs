using System;
using System.Collections.Generic;
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
    class Test03PostsViaSimpleDto
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
        public void Check01ReferenceOk()
        {

            //SETUP    

            //ATTEMPT
            ICreateService<Post, SimplePostDto> createService = new CreateService<Post, SimplePostDto>(null);
            IDetailService<Post, SimplePostDto> detailService = new DetailService<Post, SimplePostDto>(null, null);
            IListService<Post, SimplePostDto> listService = new ListService<Post, SimplePostDto>(null);
            IUpdateService<Post, SimplePostDto> updateService = new UpdateService<Post, SimplePostDto>(null);

            //VERIFY
            (listService is IListService<Post, SimplePostDto>).ShouldEqual(true);
        }

        [Test]
        public void Check02ListPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new ListService<Post, SimplePostDto>(db);

                //ATTEMPT
                var query = service.GetList();
                var list = query.ToList();

                //VERIFY
                list.Count.ShouldEqual(3);
                list[0].Title.ShouldEqual("First great post");
                list[0].BloggerName.ShouldEqual("Jon Smith");
                list[0].TagNames.ShouldEqual("Good post, Ugly post");
                list[0].LastUpdatedUtc.Kind.ShouldEqual(DateTimeKind.Utc);

            }
        }

        [Test]
        public void Check03DetailPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailService<Post, SimplePostDto>(db, new SimplePostDto());
                var firstPost = db.Posts.Include(x => x.AllocatedTags).AsNoTracking().First();

                //ATTEMPT
                var dto = service.GetDetail(x => x.PostId == firstPost.PostId);

                //VERIFY
                dto.PostId.ShouldEqual(firstPost.PostId);
                dto.BloggerName.ShouldEqual(firstPost.Blogger.Name);
                dto.Title.ShouldEqual(firstPost.Title);
                CollectionAssert.AreEqual(firstPost.AllocatedTags.Select(x => x.TagId), dto.AllocatedTags.Select(x => x.TagId));
            }
        }


        [Test]
        public void Check04ListDtoUpdateOk()
        {

            //SETUP
            var dto = new SimplePostDto
            {
                PostId = 123,
                BloggerName = "This should not be copied",
                Title = "Should copy this title",
                LastUpdated = new DateTime(2000, 1, 1)
            };
            dto.AllocatedTags = new List<PostTagLink>
                {
                    new PostTagLink {PostId = 123, HasTag = new Tag {Name = "Should not copy this", Slug = "No"}}
                };

            //ATTEMPT
            var newData = new Post
            {
                Blogger = new Blog { Name = "Original Blog Name" },
                BlogId = 777,
                Content = "Original Content"
            };
            newData.AllocatedTags = new List<PostTagLink>
                {
                    new PostTagLink {InPost = newData, HasTag = new Tag {Name = "Original Tag name", Slug = "Yes"}}
                };
            var status = dto.CopyDtoToData(null, dto, newData);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            newData.PostId.ShouldEqual(123);
            newData.Title.ShouldEqual("Should copy this title");

            newData.Blogger.Name.ShouldEqual("Original Blog Name");
            newData.BlogId.ShouldEqual(777);
            newData.Content.ShouldEqual("Original Content");
            newData.AllocatedTags.Count.ShouldEqual(1);
            newData.AllocatedTags.First().HasTag.Name.ShouldEqual("Original Tag name");
        }

        [Test]
        public void Check05UpdateSetupOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateSetupService<Post, SimplePostDto>(db, new SimplePostDto());
                var firstPost = db.Posts.Include(x => x.AllocatedTags).AsNoTracking().First();

                //ATTEMPT
                var dto = service.GetOriginal(x => x.PostId == firstPost.PostId);

                //VERIFY
                dto.PostId.ShouldEqual(firstPost.PostId);
                dto.BloggerName.ShouldEqual(firstPost.Blogger.Name);
                dto.Title.ShouldEqual(firstPost.Title);
                CollectionAssert.AreEqual(firstPost.AllocatedTags.Select(x => x.TagId), dto.AllocatedTags.Select(x => x.TagId));
            }
        }


        [Test]
        public void Check06UpdateWithListDtoOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var firstPost = db.Posts.Include(x => x.AllocatedTags).AsNoTracking().First();
                var service = new UpdateService<Post, SimplePostDto>(db);
                var setupService = new UpdateSetupService<Post, SimplePostDto>(db, new SimplePostDto());

                //ATTEMPT
                var dto = setupService.GetOriginal(x => x.PostId == firstPost.PostId);
                dto.Title = Guid.NewGuid().ToString();
                var status = service.Update(dto);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.SuccessMessage.ShouldEqual("Successfully updated Post.");
                snap.CheckSnapShot(db);
                
            }
        }

        [Test]
        public void Check07UpdateWithListDtoCorrectOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var firstPost = db.Posts.Include(x => x.AllocatedTags).AsNoTracking().First();
                var service = new UpdateService<Post, SimplePostDto>(db);
                var setupService = new UpdateSetupService<Post, SimplePostDto>(db, new SimplePostDto());

                //ATTEMPT
                var dto = setupService.GetOriginal(x => x.PostId == firstPost.PostId);
                dto.Title = Guid.NewGuid().ToString();
                var status = service.Update(dto);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                var updatedPost = db.Posts.Include(x => x.AllocatedTags).First();
                updatedPost.Title.ShouldEqual(dto.Title);
                updatedPost.Blogger.ShouldNotEqualNull();
                CollectionAssert.AreEqual(firstPost.AllocatedTags.Select(x => x.TagId), updatedPost.AllocatedTags.Select(x => x.TagId));

            }
        }

        [Test]
        public void Check08UpdateWithListDtoBad()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var listService = new ListService<Post, SimplePostDto>(db);
                var firstPostUntracked = listService.GetList().First();
                var service = new UpdateService<Post, SimplePostDto>(db);

                //ATTEMPT
                firstPostUntracked.Title = "Can't I ask a question?";
                var status = service.Update(firstPostUntracked);

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("Sorry, but you can't ask a question, i.e. the title can't end with '?'.");

            }
        }

        [Test]
        public void Check08CreateWithListDtoBad()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateService<Post, SimplePostDto>(db);

                //ATTEMPT
                var dto = new SimplePostDto();
                var status = service.Create(dto);

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEqual("Create of a new Post is not supported in this mode.");

            }
        }

    }
}
