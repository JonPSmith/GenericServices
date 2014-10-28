#region licence
// The MIT License (MIT)
// 
// Filename: Test03CheckGenericDtoAsync.cs
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
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using GenericServices.Core;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group09CrudServicesAsync
{
    class Test03CheckGenericDtoAsync
    {
        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            using (var db = new SampleWebAppDb())
            {
                DataLayerInitialise.InitialiseThis();
                var filepath = TestFileHelpers.GetTestFileFilePath("DbContentSimple.xml");
                DataLayerInitialise.ResetDatabaseToTestData(db, filepath);
            }
        }

        [Test]
        public async void Check01CreateDataFromDtoAsyncOk()
        {

            //SETUP
            var dto = new SimplePostDtoAsync
            {
                PostId = 123,
                BloggerName = "This should not be copied",
                Title = "Should copy this title",
                LastUpdated = new DateTime(2000, 1, 1),
                Tags = new Collection<Tag> { new Tag { Name = "Should not copy this", Slug = "No" } }
            };

            var status = await dto.CreateDataFromDtoAsync(null, dto);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.PostId.ShouldEqual(123);
            status.Result.Title.ShouldEqual("Should copy this title");

            status.Result.Blogger.ShouldEqual(null);
            status.Result.BlogId.ShouldEqual(0);
            status.Result.Content.ShouldEqual(null);
            status.Result.Tags.ShouldEqual(null);
        }

        [Test]
        public async void Check02UpdateDataFromDtoAsyncOk()
        {

            //SETUP
            var dto = new SimplePostDtoAsync
            {
                PostId = 123,
                BloggerName = "This should not be copied",
                Title = "Should copy this title",
                LastUpdated = new DateTime(2000, 1, 1),
                Tags = new Collection<Tag> { new Tag { Name = "Should not copy this", Slug = "No" } }
            };


            //ATTEMPT
            var newData = new Post
            {
                Blogger = new Blog { Name = "Original Blog Name" },
                BlogId = 777,
                Content = "Original Content",
                Tags = new Collection<Tag> { new Tag { Name = "Original Tag name", Slug = "Yes" } }
            };

            var status = await dto.UpdateDataFromDtoAsync(null, dto, newData);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            newData.PostId.ShouldEqual(123);
            newData.Title.ShouldEqual("Should copy this title");

            newData.Blogger.Name.ShouldEqual("Original Blog Name");
            newData.BlogId.ShouldEqual(777);
            newData.Content.ShouldEqual("Original Content");
            newData.Tags.Count.ShouldEqual(1);
            newData.Tags.First().Name.ShouldEqual("Original Tag name");
        }

        //--------------------------

        [Test]
        public async void Check06CreateDtoAndCopyInDataOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP;
                var firstPost = db.Posts.Include( x => x.Blogger).Include(x => x.Tags).AsNoTracking().First();

                //ATTEMPT
                var status = await new SimplePostDtoAsync().DetailDtoFromDataInAsync(db, x => x.PostId == firstPost.PostId);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.PostId.ShouldEqual(firstPost.PostId);
                status.Result.Title.ShouldEqual(firstPost.Title);
                status.Result.LastUpdated.ShouldEqual(firstPost.LastUpdated);
                status.Result.LastUpdatedUtc.Kind.ShouldEqual(DateTimeKind.Utc);

                status.Result.BloggerName.ShouldEqual(firstPost.Blogger.Name);
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.TagId), status.Result.Tags.Select(x => x.TagId));
            }
        }
    }
}
