#region licence
// The MIT License (MIT)
// 
// Filename: Test02UpdateService.cs
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

using System;
using System.Linq;
using GenericServices.Core;
using GenericServices.Services.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group03ServiceFlow
{
    class Test02UpdateService
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
        public void Check01UpdateSetupFlowOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateSetupService<Tag, SimpleTagDto>(db);
                var firstTag = db.Tags.First();

                //ATTEMPT
                var status = service.GetOriginal(firstTag.TagId);

                //VERIFY
                status.IsValid.ShouldEqual(true, status.Errors);
                status.Result.FunctionsCalledCommaDelimited.ShouldEqual("DetailDtoFromDataIn,SetupSecondaryData");
            }
        }

        [Test]
        public void Check02UpdateFailOk()
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateService<Tag, SimpleTagDto>(db);
                var dto = new SimpleTagDto();
                dto.SetSupportedFunctions(ServiceFunctions.None);

                //ATTEMPT
                var status = service.Update(dto);

                //VERIFY
                status.IsValid.ShouldEqual(false);
                status.Errors.Count.ShouldEqual(1);
                status.Errors[0].ErrorMessage.ShouldEndWith("is not supported in this mode.");
                dto.FunctionsCalledCommaDelimited.ShouldEqual("");
            }
        }

        [TestCase(InstrumentedOpFlags.NormalOperation, true, "FindItemTrackedForUpdate,UpdateDataFromDto")]
        [TestCase(InstrumentedOpFlags.FailOnUpdateDataFromDto, false, "FindItemTrackedForUpdate,UpdateDataFromDto,SetupSecondaryData")]
        public void Check02UpdateFlow(InstrumentedOpFlags errorFlag, bool isValid, string expectedFunctionsCalled)
        {
            using (var db = new SampleWebAppDb())
            {
                //SETUP
                var service = new UpdateService<Tag, SimpleTagDto>(db);
                var dto = new SimpleTagDto(errorFlag)
                {
                    TagId = db.Tags.First().TagId,
                    Name = "Test Name",
                    Slug = Guid.NewGuid().ToString("N")
                };

                //ATTEMPT
                //dto.CopyDtoToData(db, dto, new Tag());          //do this to preload AutoMapper
                var status = service.Update(dto);

                //VERIFY
                status.IsValid.ShouldEqual(isValid);
                dto.FunctionsCalledCommaDelimited.ShouldEqual(expectedFunctionsCalled);
            }
        }  

    }
}
