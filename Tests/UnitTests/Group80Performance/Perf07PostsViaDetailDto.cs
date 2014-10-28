#region licence
// The MIT License (MIT)
// 
// Filename: Perf07PostsViaDetailDto.cs
// Date Created: 2014/06/12
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
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group80Performance
{
    class Perf07PostsViaDetailDto
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
        public void Perf03DetailPostOk()
        {
            int postId;
            using (var db = new SampleWebAppDb())
                postId = db.Posts.Include(x => x.Tags).AsNoTracking().First().PostId;

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailService<Post, DetailPostDto>(db);

                //ATTEMPT
                var status = service.GetDetailUsingWhere(x => x.PostId == postId);
                status.Result.LogSpecificName("End");

                //VERIFY
                foreach (var log in status.Result.LogOfCalls) { Console.WriteLine(log); }
            }
        }

        [Test]
        public void Perf10CreateSetupServiceOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new CreateSetupService<Post, DetailPostDto>(db);

                //ATTEMPT
                var dto = service.GetDto();
                dto.LogSpecificName("End");

                //VERIFY
                foreach (var log in dto.LogOfCalls) { Console.WriteLine(log); }
            }
        }


        [Test]
        public void Perf11CreatePostOk()
        {

            using (var db = new SampleWebAppDb())
            {
                //SETUP

                var service = new CreateService<Post, DetailPostDto>(db);
                var setupService = new CreateSetupService<Post, DetailPostDto>(db);

                //ATTEMPT
                var dto = setupService.GetDto();
                dto.Title = Guid.NewGuid().ToString();
                dto.Content = "something to fill it as can't be empty";
                dto.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                dto.UserChosenTags.FinalSelection = db.Tags.Take(2).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = service.Create(dto);
                dto.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                foreach (var log in dto.LogOfCalls) { Console.WriteLine(log); }
            }
        }



        [Test]
        public void Perf16UpdateSetupServiceOk()
        {
            int postId;
            using (var db = new SampleWebAppDb())
                postId = db.Posts.Include(x => x.Tags).AsNoTracking().First().PostId;

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var setupService = new UpdateSetupService<Post, DetailPostDto>(db);

                //ATTEMPT
                var status = setupService.GetOriginal(postId);
                status.Result.LogSpecificName("End");

                //VERIFY
                foreach (var log in status.Result.LogOfCalls) { Console.WriteLine(log); }
            }
        }



        [Test]
        public void Perf22UpdatePostAddTagOk()
        {
            int postId;
            using (var db = new SampleWebAppDb())
                postId = db.Posts.Include(x => x.Tags).AsNoTracking().First().PostId;

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var setupService = new UpdateSetupService<Post, DetailPostDto>(db);
                var updateService = new UpdateService<Post, DetailPostDto>(db);

                //ATTEMPT
                var setupStatus = setupService.GetOriginal(postId);
                setupStatus.Result.Title = Guid.NewGuid().ToString();
                setupStatus.Result.Bloggers.SelectedValue = db.Blogs.First().BlogId.ToString("D");
                setupStatus.Result.UserChosenTags.FinalSelection = db.Tags.Take(3).ToList().Select(x => x.TagId.ToString("D")).ToArray();
                var status = updateService.Update(setupStatus.Result);
                setupStatus.Result.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                foreach (var log in setupStatus.Result.LogOfCalls) { Console.WriteLine(log); }
            }
        }

    }
}
