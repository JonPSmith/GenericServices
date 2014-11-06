#region licence
// The MIT License (MIT)
// 
// Filename: Test03CheckGenericDto.cs
// Date Created: 2014/05/26
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
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group08CrudServices
{
    class Test03CheckGenericDto
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
        public void Test01CreateDataFromDtoOk()
        {

            //SETUP
            var dto = new SimplePostDto
            {
                PostId = 123,
                BloggerName = "This should not be copied",
                Title = "Should copy this title",
                LastUpdated = new DateTime(2000, 1, 1),
                Tags = new Collection<Tag> { new Tag { Name = "Should not copy this", Slug = "No" } }
            };


            //ATTEMPT
            var status = dto.CreateDataFromDto(null, dto);

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
        public void Test02UpdateDataFromDtoOk()
        {

            //SETUP
            var dto = new SimplePostDto
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

            var status = dto.UpdateDataFromDto(null, dto, newData);

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


        [Test]
        public void Test06CreateDtoAndCopyInDataOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP;
                var firstPost = db.Posts.Include( x => x.Blogger).Include(x => x.Tags).AsNoTracking().First();

                //ATTEMPT
                var status = new SimplePostDto().DetailDtoFromDataIn(db, x => x.PostId == firstPost.PostId);

                //VERIFY
                status.Result.PostId.ShouldEqual(firstPost.PostId);
                status.Result.Title.ShouldEqual(firstPost.Title);
                status.Result.LastUpdated.ShouldEqual(firstPost.LastUpdated);
                status.Result.LastUpdatedUtc.Kind.ShouldEqual(DateTimeKind.Utc);

                status.Result.BloggerName.ShouldEqual(firstPost.Blogger.Name);
                CollectionAssert.AreEqual(firstPost.Tags.Select(x => x.TagId), status.Result.Tags.Select(x => x.TagId));
            }
        }

        //-----------------------------------------------------------------------------
        //check override mappings

        [Test]
        public void Test10CreateDtoAndCopyInDataMappingOverriddenOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP;
                var firstPost = db.Posts.Include(x => x.Blogger).Include(x => x.Tags).AsNoTracking().First();

                //ATTEMPT
                var status = new PostSpecialMappingDto().DetailDtoFromDataIn(db, x => x.PostId == firstPost.PostId);

                //VERIFY
                status.Result.PostId.ShouldEqual(firstPost.PostId);
                status.Result.Title.ShouldEqual(firstPost.Title);
                status.Result.Content.ShouldEqual(firstPost.Content);

                status.Result.BloggerInfo.ShouldEqual(firstPost.Blogger.Name + " " + firstPost.Blogger.EmailAddress);
                status.Result.CountOfTags.ShouldEqual(firstPost.Tags.Count);

                CollectionAssert.AreEqual( firstPost.Tags.Select( x => x.Name), status.Result.Tags.Select( x => x.Name));
            }
        }


        [Test]
        public void Test11CreateDataFromDtoMappingOverriddenOk()
        {

            //SETUP
            var dto = new PostSpecialMappingDto
            {
                PostId = 123,
                BloggerInfo = "This should not be copied",
                Title = "Should copy this title",
                Content = "Should copy content",
            };


            //ATTEMPT
            var status = dto.CreateDataFromDto(null, dto);

            //VERIFY
            status.ShouldBeValid();
            //should be copied or altered
            status.Result.Title.ShouldEqual("Should copy this title prefix to title");
            status.Result.Content.ShouldEqual("Should copy content");
            
            //should not be copied
            status.Result.PostId.ShouldEqual(0);
            status.Result.LastUpdated.Ticks.ShouldEqualWithTolerance(DateTime.UtcNow.Ticks, 100000000);
            status.Result.Blogger.ShouldEqual(null);
            status.Result.BlogId.ShouldEqual(0);
            status.Result.Tags.ShouldEqual(null);
        }

        [Test]
        public void Test20CheckAssociatedMappingsBad()
        {

            //SETUP

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => new BadSpecialMappingDto());

            //VERIFY
            ex.Message.ShouldEqual("You have not supplied a class based on EfGenericDto to set up the mapping.");
        }
    }
}
