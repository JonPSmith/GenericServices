#region licence
// The MIT License (MIT)
// 
// Filename: Test02DecodeToService.cs
// Date Created: 2014/07/19
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
using System.Diagnostics;
using GenericServices.Core.Internal;
using GenericServices.Services.Concrete;
using GenericServices.ServicesAsync.Concrete;
using NUnit.Framework;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group15CrudServiceFinder
{
    class Test02DecodeToService
    {

        //[TestFixtureSetUp]
        //public void FixtureSetup()
        //{
        //    DecodeToService<ListService>.CreateCorrectService<Post>(WhatItShouldBe.SyncAnything, new object[] { null });     //sets up the initial constants
        //}

        [Test]
        public void Check00DecodeListOk()
        {

            //SETUP
            var timer = new Stopwatch();          

            //ATTEMPT
            timer.Start();
            var service = DecodeToService<ListService>.CreateCorrectService<Post>(WhatItShouldBe.SyncAnything, new object[] { null });
            timer.Stop();

            //VERIFY
            ExtendAsserts.ShouldNotEqualNull(service);
            ExtendAsserts.IsA<ListService<Post>>(service);
            Console.WriteLine("took {0:f3} ms", 1000.0 * timer.ElapsedTicks / Stopwatch.Frequency);
        }

        //--------------------------------------------------------
        //errors

        [Test]
        public void Check01DecodeNoSyncAsyncBad()
        {

            //SETUP

            //ATTEMPT

            var ex = Assert.Throws<InvalidOperationException>(
                () => DecodeToService<ListService>.CreateCorrectService<Post>(WhatItShouldBe.DataClass, new object[] { null }));

            //VERIFY
            ex.Message.ShouldEqual("Neither the IsSync or the IsAsync flags were set.");
        }

        [Test]
        public void Check02DecodeListMustBeDtoBad()
        {

            //SETUP

            //ATTEMPT

            var ex = Assert.Throws<InvalidOperationException>(
                () => DecodeToService<ListService>.CreateCorrectService<Post>(WhatItShouldBe.SyncAnyDto, new object[] { null }));

            //VERIFY
            ex.Message.ShouldEqual("This type of service only works with some form of EfGenericDto.");
        }

        [Test]
        public void Check03MustBeSpecificDtoBad()
        {

            //SETUP

            //ATTEMPT

            var ex = Assert.Throws<InvalidOperationException>(
                () => DecodeToService<DetailServiceAsync>.CreateCorrectService<SimplePostDto>(WhatItShouldBe.AsyncSpecificDto, new object[] { null }));

            //VERIFY
            ex.Message.ShouldEqual("This service needs a class which inherited from EfGenericDtoAsync`2.");
        }

        [Test]
        public void Check04MustBeSpecificDtoBad()
        {

            //SETUP
            var timer = new Stopwatch();

            //ATTEMPT

            var ex = Assert.Throws<InvalidOperationException>(
                () => DecodeToService<DetailService>.CreateCorrectService<SimplePostDtoAsync>(WhatItShouldBe.SyncSpecificDto, new object[] { null }));

            //VERIFY
            ex.Message.ShouldEqual("This service needs a class which inherited from EfGenericDto`2.");
        }

        //------------------------------------------------------------------


        [Test]
        public void Check05DecodeListDtoOk()
        {

            //SETUP
            dynamic service;

            //ATTEMPT
            using(new TimerToConsole("Decode"))
                service = DecodeToService<ListService>.CreateCorrectService<SimplePostDto>(WhatItShouldBe.SyncAnything, new object[] { null });

            //VERIFY
            ExtendAsserts.ShouldNotEqualNull(service);
            ExtendAsserts.IsA<ListService<Post,SimplePostDto>>(service);
        }

        [Test]
        public void Check06DecodeListDtoAsyncOk()
        {

            //SETUP
            dynamic service;

            //ATTEMPT
            using(new TimerToConsole("Decode"))
                service = DecodeToService<ListService>.CreateCorrectService<SimplePostDtoAsync>(WhatItShouldBe.SyncAnything, new object[] { null });

            //VERIFY
            ExtendAsserts.ShouldNotEqualNull(service);
            ExtendAsserts.IsA<ListService<Post,SimplePostDtoAsync>>(service);
        }

        [Test]
        public void Check10DecodeDetailOk()
        {

            //SETUP
            dynamic service;

            //ATTEMPT
            using (new TimerToConsole("Decode"))
                service = DecodeToService<DetailService>.CreateCorrectService<Post>(WhatItShouldBe.SyncAnything, new object[] { null });

            //VERIFY
            ExtendAsserts.ShouldNotEqualNull(service);
            ExtendAsserts.IsA<DetailService<Post>>(service);
        }

        [Test]
        public void Check11DecodeDetailDtoOk()
        {

            //SETUP
            dynamic service;

            //ATTEMPT
            using(new TimerToConsole("Decode"))
                service = DecodeToService<DetailService>.CreateCorrectService<SimplePostDto>(WhatItShouldBe.SyncAnything, new object[] { null });

            //VERIFY
            ExtendAsserts.ShouldNotEqualNull(service);
            ExtendAsserts.IsA<DetailService<Post,SimplePostDto>>(service);
        }

        [Test]
        public void Check12DecodeDetailAsyncOk()
        {

            //SETUP
            dynamic service;

            //ATTEMPT
            using (new TimerToConsole("Decode"))
                service = DecodeToService<DetailServiceAsync>.CreateCorrectService<Post>(WhatItShouldBe.AsyncAnything, new object[] { null });;

            //VERIFY
            ExtendAsserts.ShouldNotEqualNull(service);
            ExtendAsserts.IsA<DetailServiceAsync<Post>>(service);
        }

        [Test]
        public void Check13DecodeDetailDtoAsyncOk()
        {

            //SETUP
            dynamic service;

            //ATTEMPT
            using (new TimerToConsole("Decode"))
                service = DecodeToService<DetailServiceAsync>.CreateCorrectService<SimplePostDtoAsync>(WhatItShouldBe.AsyncAnything, new object[] { null });

            //VERIFY
            ExtendAsserts.ShouldNotEqualNull(service);
            ExtendAsserts.IsA<DetailServiceAsync<Post,SimplePostDtoAsync>>(service);
        }

    }
}
