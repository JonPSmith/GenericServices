#region licence
// The MIT License (MIT)
// 
// Filename: Test10ActionService.cs
// Date Created: 2014/05/24
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

using GenericServices.Core;
using GenericServices.Services.Concrete;
using NUnit.Framework;
using Tests.Actions;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group03ServiceFlow
{
    class Test10ActionService
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
        public void Check01ActionNoSubmitOk()
        {

            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var service = new ActionService<int, Tag, SimpleTagDto>(dummyDb, new EmptyTestAction(false));
            var dto = new SimpleTagDto {Name = "test", Slug = "test"};

            //ATTEMPT
            var status = service.DoAction(dto);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            dummyDb.SaveChangesCalled.ShouldEqual(false);

        }

        [Test]
        public void Check02ActionSubmitOk()
        {

            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var service = new ActionService<int, Tag, SimpleTagDto>(dummyDb, new EmptyTestAction(true));
            var dto = new SimpleTagDto { Name = "test", Slug = "test" };

            //ATTEMPT
            var status = service.DoAction(dto);

            //VERIFY
            status.IsValid.ShouldEqual(true);
            dummyDb.SaveChangesCalled.ShouldEqual(true);

        }

        [Test]
        public void Check03ActionFailOk()
        {

            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var service = new ActionService<int, Tag, SimpleTagDto>(dummyDb, new EmptyTestAction(false));
            var dto = new SimpleTagDto();
            dto.SetSupportedFunctions(ServiceFunctions.None);

            //ATTEMPT
            var status = service.DoAction(dto);

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("Running an action is not setup for this data.");
            dto.FunctionsCalledCommaDelimited.ShouldEqual("");

        }

        [TestCase(InstrumentedOpFlags.NormalOperation, true, "CreateDataFromDto")]
        [TestCase(InstrumentedOpFlags.FailOnCreateDataFromDto, false, "CreateDataFromDto")]
        [TestCase(InstrumentedOpFlags.ForceActionFail, false, "CreateDataFromDto")]
        [TestCase(InstrumentedOpFlags.ForceActionWarnWithWrite, true, "CreateDataFromDto")]
        public void Check05ActionFlowAction(InstrumentedOpFlags errorFlag, bool isValid, string expectedFunctionsCalled)
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var service = new ActionService<int, Tag, SimpleTagDto>(dummyDb, new EmptyTestAction(false));
            var dto = new SimpleTagDto(errorFlag){Name = "test", Slug = "test"};
            if (errorFlag == InstrumentedOpFlags.ForceActionFail)
                dto.TagId = 2;
            else if (errorFlag == InstrumentedOpFlags.ForceActionWarnWithWrite)
                dto.TagId = 1;

            //ATTEMPT
            var status = service.DoAction(dto);

            //VERIFY
            status.IsValid.ShouldEqual(isValid, status.Errors);
            status.Result.ShouldEqual(status.IsValid ? dto.TagId : 0);
            dto.FunctionsCalledCommaDelimited.ShouldEqual(expectedFunctionsCalled);
        }

    }
}
