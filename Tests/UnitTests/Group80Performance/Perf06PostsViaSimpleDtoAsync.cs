#region licence
// The MIT License (MIT)
// 
// Filename: Perf06PostsViaSimpleDtoAsync.cs
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
using GenericServices.ServicesAsync.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group80Performance
{
    class Perf06PostsViaSimpleDtoAsync
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
            new SimplePostDto().CacheSetup();
        }
        
        //--------------------------------------------------------


        [Test]
        public async void Perf01DetailPostOk()
        {
            int postId;
            using (var db = new SampleWebAppDb())
                postId = db.Posts.Include(x => x.Tags).AsNoTracking().First().PostId;

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new DetailServiceAsync<Post, SimplePostDtoAsync>(db);

                //ATTEMPT
                var status = await service.GetDetailAsync(postId);
                status.Result.LogSpecificName("End");

                //VERIFY
                foreach (var log in status.Result.LogOfCalls) { Console.WriteLine(log); }
            }
        }

        [Test]
        public async void Perf05UpdateSetupOk()
        {
            int postId;
            using (var db = new SampleWebAppDb())
                postId = db.Posts.Include(x => x.Tags).AsNoTracking().First().PostId;

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateSetupServiceAsync<Post, SimplePostDtoAsync>(db);

                //ATTEMPT
                var status = await service.GetOriginalAsync(postId);
                status.Result.LogSpecificName("End");

                //VERIFY
                foreach (var log in status.Result.LogOfCalls) { Console.WriteLine(log); }
            }
        }

        [Test]
        public async void Perf06UpdateWithListDtoOk()
        {
            int postId;
            using (var db = new SampleWebAppDb())
                postId = db.Posts.Include(x => x.Tags).AsNoTracking().First().PostId;

            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateServiceAsync<Post, SimplePostDtoAsync>(db);
                var setupService = new UpdateSetupServiceAsync<Post, SimplePostDtoAsync>(db);

                //ATTEMPT
                var setupStatus = await setupService.GetOriginalAsync(postId);
                setupStatus.Result.Title = Guid.NewGuid().ToString();
                var status = await service.UpdateAsync(setupStatus.Result);
                setupStatus.Result.LogSpecificName("End");

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                foreach (var log in setupStatus.Result.LogOfCalls) { Console.WriteLine(log); }
                
            }
        }


    }
}
