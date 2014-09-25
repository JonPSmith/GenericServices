#region licence
// The MIT License (MIT)
// 
// Filename: Test05ActionServiceEmptyActionAsync.cs
// Date Created: 2014/06/26
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
using GenericServices.ServicesAsync;
using GenericServices.ServicesAsync.Concrete;
using NUnit.Framework;
using Tests.Actions;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group20Actions
{
    class Test05ActionServiceEmptyActionAsync
    {

        [Test]
        public void Check01ReferencesOk()
        {

            //SETUP    

            //ATTEMPT
            IActionServiceAsync<int, Tag> actionService = new ActionServiceAsync<int, Tag>(null, new EmptyTestActionAsync(false));
            IActionServiceAsync<int, Tag, SimpleTagDtoAsync> actionDtoService =
                new ActionServiceAsync<int, Tag, SimpleTagDtoAsync>(null, new EmptyTestActionAsync(false));

            //VERIFY
            (actionService is ActionServiceAsync<int, Tag>).ShouldEqual(true);
        }

        [Test]
        public async void Check02RunActionServiceSuccessOk()
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestActionAsync(false);
            var service = new ActionServiceAsync<int, Tag>(dummyDb, testAction);

            //ATTEMPT
            var data = new Tag
            {
                TagId = -456
            };
            var status = await service.DoActionAsync(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.ShouldEqual(-456);
            dummyDb.SaveChangesCalled.ShouldEqual(false);
        }

        [Test]
        public async void Check03RunActionServiceCallSubmitOk()
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestActionAsync(true);
            var service = new ActionServiceAsync<int, Tag>(dummyDb, testAction);

            //ATTEMPT
            var data = new Tag();
            var status = await service.DoActionAsync(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.HasWarnings.ShouldEqual(false);
            dummyDb.SaveChangesCalled.ShouldEqual(true);
            status.SuccessMessage.ShouldEndWith("... and written to database.");
        }

        [Test]
        public async void Check04RunActionServiceWarningsButSumbitOk()
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestActionAsync(true);
            var service = new ActionServiceAsync<int, Tag>(dummyDb, testAction);

            //ATTEMPT
            var data = new Tag(true)
            {
                TagId = 1 //means a warning
            };
            var status = await service.DoActionAsync(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.HasWarnings.ShouldEqual(true);
            dummyDb.SaveChangesCalled.ShouldEqual(true);
            status.SuccessMessage.ShouldEndWith("... and written to database. (has 1 warnings)");
        }

        [Test]
        public async void Check05RunActionServiceWarningsNoSubmitOk()
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestActionAsync(true);
            var service = new ActionServiceAsync<int, Tag>(dummyDb, testAction);

            //ATTEMPT
            var data = new Tag()
            {
                TagId = 1 //means a warning
            };
            var status = await service.DoActionAsync(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.HasWarnings.ShouldEqual(true);
            dummyDb.SaveChangesCalled.ShouldEqual(false);
            status.SuccessMessage.ShouldEndWith("... but NOT written to database as warnings. (has 1 warnings)");
        }

        [Test]
        public async void Check06RunActionServiceFailNoSubmitOk()
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestActionAsync(true);
            var service = new ActionServiceAsync<int, Tag>(dummyDb, testAction);

            //ATTEMPT
            var data = new Tag
            {
                TagId = 3 //will fail
            };
            var status = await service.DoActionAsync(data);

            //VERIFY
            status.IsValid.ShouldEqual(false, status.Errors);
            dummyDb.SaveChangesCalled.ShouldEqual(false);
        }

        [Test]
        public async void Check10RunActionDtoSuccessOk()
        {
            //SETUP  
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestActionAsync(false);
            var service = new ActionServiceAsync<int, Tag, SimpleTagDtoAsync>(dummyDb, testAction);

            //ATTEMPT
            var dto = new SimpleTagDtoAsync
            {
                TagId = -123,
                Name = "test",
                Slug = "test"
            };
            var status = await service.DoActionAsync(dto);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.ShouldEqual(-123);
            dummyDb.SaveChangesCalled.ShouldEqual(false);
        }

        [Test]
        public async void Check11RunActionDtoSubmitOk()
        {
            //SETUP  
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestActionAsync(true);
            var service = new ActionServiceAsync<int, Tag, SimpleTagDtoAsync>(dummyDb, testAction);

            //ATTEMPT
            var dto = new SimpleTagDtoAsync
            {
                TagId = -123,
                Name = "test",
                Slug = "test"
            };
            var status = await service.DoActionAsync(dto);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.ShouldEqual(-123);
            dummyDb.SaveChangesCalled.ShouldEqual(true);
            status.SuccessMessage.ShouldEndWith("... and written to database.");
        }


        [Test] 
        public async void Check12RunActionDtoFailNoSubmitOk()
        {
            //SETUP  
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestActionAsync(true);
            var service = new ActionServiceAsync<int, Tag, SimpleTagDtoAsync>(dummyDb, testAction);

            //ATTEMPT
            var dto = new SimpleTagDtoAsync
            {
                TagId = 3,           //will fail
                Name = "test",
                Slug = "test"
            };
            var status = await service.DoActionAsync(dto);

            //VERIFY
            status.IsValid.ShouldEqual(false, status.Errors);
            dummyDb.SaveChangesCalled.ShouldEqual(false);
        }


    }
}
