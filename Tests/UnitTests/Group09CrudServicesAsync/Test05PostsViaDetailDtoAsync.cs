#region licence
// The MIT License (MIT)
// 
// Filename: Test05PostsViaDetailDtoAsync.cs
// Date Created: 2014/06/17
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
using System.Linq;
using GenericServices.Services.Concrete;
using GenericServices.ServicesAsync;
using GenericServices.ServicesAsync.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group09CrudServicesAsync
{
    class Test05PostsViaDetailDtoAsync
    {

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            new DetailPostDtoAsync().CacheSetup();
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
        public async void Check01ReferencesOk()
        {

            //SETUP    

            //ATTEMPT
            ICreateServiceAsync<Post, DetailPostDtoAsync> createService = new CreateServiceAsync<Post, DetailPostDtoAsync>(null);
            IDetailServiceAsync<Post, DetailPostDtoAsync> detailService = new DetailServiceAsync<Post, DetailPostDtoAsync>(null);
            IUpdateServiceAsync<Post, DetailPostDtoAsync> updateService = new UpdateServiceAsync<Post, DetailPostDtoAsync>(null);

            //VERIFY
            (createService is CreateServiceAsync<Post, DetailPostDtoAsync>).ShouldEqual(true);
        }

        [Test]
        public async void Check02ListPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new ListService<Post, DetailPostDtoAsync>(db);

                //ATTEMPT
                var list = await service.GetAll().ToListAsync();

                //VERIFY
                list.Count().ShouldEqual(3);
                var firstPost = db.Posts.Include(x => x.Tags).ToList().First(x => x.PostId == list.First().PostId);
                list.First().Title.ShouldEqual(firstPost.Title);
                list.First().Content.ShouldEqual(firstPost.Content);
                list.First().BloggerName.ShouldEqual(firstPost.Blogger.Name);
                list.First().TagNames.ShouldEqual("Ugly post, Good post");
                list.First().LastUpdatedUtc.Kind.ShouldEqual(DateTimeKind.Utc);
            }
        }

        [Test]
        public async void Check03DetailPostOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailServiceAsync<Post, DetailPostDtoAsync>(db);
                var firstPost = db.Posts.Include( x => x.Tags).AsNoTracking().First();

                //ATTEMPT
                var status = await service.GetDetailAsync(firstPost.PostId);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.PostId.ShouldEqual(firstPost.PostId);
                status.Result.BlogId.ShouldEqual(firstPost.BlogId);
                status.Result.BloggerName.ShouldEqual(firstPost.Blogger.Name);
                status.Result.Title.ShouldEqual(firstPost.Title);
                status.Result.Content.ShouldEqual(firstPost.Content);
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.TagId), status.Result.Tags.Select(x => x.TagId));
            }
        }

        [Test]
        public async void Check10CreateSetupServiceOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateSetupServiceAsync<Post, DetailPostDtoAsync>(db);

                //ATTEMPT
                var dto = await service.GetDtoAsync();

                //VERIFY
                dto.Bloggers.KeyValueList.Count.ShouldEqual( db.Blogs.Count()+1);
                dto.UserChosenTags.AllPossibleOptions.Count.ShouldEqual( db.Tags.Count());
            }
        }


        [Test]
        public async void Check11CreatePostOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var service = new CreateServiceAsync<Post, DetailPostDtoAsync>(db);
                var setupService = new CreateSetupServiceAsync<Post, DetailPostDtoAsync>(db);

                //ATTEMPT
                var dto = await setupService.GetDtoAsync();
                dto.Title = Guid.NewGuid().ToString();
                dto.Content = "something to fill it as can't be empty";
                dto.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                dto.UserChosenTags.FinalSelection = db.Tags.OrderBy(x => x.TagId).Take(2).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = await service.CreateAsync(dto);

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
        public async void Check12CreateFailRunsSetupSecondaryDataAgainOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateServiceAsync<Post, DetailPostDtoAsync>(db);
                var setupService = new CreateSetupServiceAsync<Post, DetailPostDtoAsync>(db);

                //ATTEMPT
                var dto = await setupService.GetDtoAsync();
                dto.Title = Guid.NewGuid().ToString();
                dto.Content = null;                                 //this will fail 
                dto.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                dto.UserChosenTags.FinalSelection = db.Tags.Take(2).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = await service.CreateAsync(dto);

                //VERIFY
                status.IsValid.ShouldEqual(false);
                dto.Bloggers.KeyValueList.Count.ShouldEqual(db.Blogs.Count() + 1);
                dto.UserChosenTags.AllPossibleOptions.Count.ShouldEqual(db.Tags.Count());
            }
        }

        [Test]
        public async void Check13CreateServiceResetWorksOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateServiceAsync<Post, DetailPostDtoAsync>(db);

                //ATTEMPT
                var dto = new DetailPostDtoAsync();
                await service.ResetDtoAsync(dto);

                //VERIFY
                dto.Bloggers.KeyValueList.Count.ShouldEqual(db.Blogs.Count() + 1);
                dto.UserChosenTags.AllPossibleOptions.Count.ShouldEqual(db.Tags.Count());
            }
        }


        [Test]
        public async void Check16UpdateSetupServiceOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var setupService = new UpdateSetupServiceAsync<Post, DetailPostDtoAsync>(db);
                var firstPost = db.Posts.First();

                //ATTEMPT
                var status = await setupService.GetOriginalAsync(firstPost.PostId);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.Bloggers.KeyValueList.Count.ShouldEqual(db.Blogs.Count() + 1);
                status.Result.UserChosenTags.AllPossibleOptions.Count.ShouldEqual(db.Tags.Count());
            }
        }

        [Test]
        public async void Check20UpdatePostLeaveTagSameOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var setupService = new UpdateSetupServiceAsync<Post, DetailPostDtoAsync>(db);
                var updateService = new UpdateServiceAsync<Post, DetailPostDtoAsync>(db);
                var firstPost = db.Posts.Include(x => x.Tags).First();

                //ATTEMPT
                var setupStatus = await setupService.GetOriginalAsync(firstPost.PostId);
                setupStatus.IsValid.ShouldEqual(true, setupStatus.Errors);
                setupStatus.Result.Title = Guid.NewGuid().ToString();
                setupStatus.Result.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                setupStatus.Result.UserChosenTags.FinalSelection = firstPost.Tags.Select(x => x.TagId.ToString("D")).ToArray();
                var status = await updateService.UpdateAsync(setupStatus.Result);

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
        public async void Check21UpdatePostRemoveTagOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var setupService = new UpdateSetupServiceAsync<Post, DetailPostDtoAsync>(db);
                var updateService = new UpdateServiceAsync<Post, DetailPostDtoAsync>(db);
                var firstPost = db.Posts.First();

                //ATTEMPT
                var setupStatus = await setupService.GetOriginalAsync(firstPost.PostId);
                setupStatus.IsValid.ShouldEqual(true, setupStatus.Errors);
                setupStatus.Result.Title = Guid.NewGuid().ToString();
                setupStatus.Result.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                setupStatus.Result.UserChosenTags.FinalSelection = db.Tags.Take(1).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = await updateService.UpdateAsync(setupStatus.Result);

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
        public async void Check22UpdatePostAddTagOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var snap = new DbSnapShot(db);
                var setupService = new UpdateSetupServiceAsync<Post, DetailPostDtoAsync>(db);
                var updateService = new UpdateServiceAsync<Post, DetailPostDtoAsync>(db);
                var firstPost = db.Posts.First();

                //ATTEMPT
                var setupStatus = await setupService.GetOriginalAsync(firstPost.PostId);
                setupStatus.IsValid.ShouldEqual(true, setupStatus.Errors);
                setupStatus.Result.Title = Guid.NewGuid().ToString();
                setupStatus.Result.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                setupStatus.Result.UserChosenTags.FinalSelection = db.Tags.Take(3).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = await updateService.UpdateAsync(setupStatus.Result);

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
        public async void Check25UpdatePostFailRunsSetupSecondaryDataAgainOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var setupService = new UpdateSetupServiceAsync<Post, DetailPostDtoAsync>(db);
                var updateService = new UpdateServiceAsync<Post, DetailPostDtoAsync>(db);
                var firstPost = db.Posts.First();

                //ATTEMPT
                var setupStatus = await setupService.GetOriginalAsync(firstPost.PostId);
                setupStatus.IsValid.ShouldEqual(true, setupStatus.Errors);
                setupStatus.Result.Title = null;                   //that will fail
                setupStatus.Result.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                setupStatus.Result.UserChosenTags.FinalSelection = db.Tags.Take(3).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = await updateService.UpdateAsync(setupStatus.Result);

                //VERIFY
                status.IsValid.ShouldEqual(false);
                setupStatus.Result.Bloggers.KeyValueList.Count.ShouldEqual(db.Blogs.Count() + 1);
                setupStatus.Result.UserChosenTags.AllPossibleOptions.Count.ShouldEqual(db.Tags.Count());
            }
        }

        [Test]
        public async void Check26UpdateServiceResetWorksOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateServiceAsync<Post, DetailPostDtoAsync>(db);

                //ATTEMPT
                var dto = new DetailPostDtoAsync();
                await service.ResetDtoAsync(dto);

                //VERIFY
                dto.Bloggers.KeyValueList.Count.ShouldEqual(db.Blogs.Count() + 1);
                dto.UserChosenTags.AllPossibleOptions.Count.ShouldEqual(db.Tags.Count());
            }
        }
    }
}
