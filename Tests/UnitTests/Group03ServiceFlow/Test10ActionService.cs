using GenericServices.Services;
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
        public void Check01ActionFailOk()
        {

            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var service = new ActionService<IEmptyTestAction, int, Tag, SimpleTagDto>(dummyDb, new EmptyTestAction());
            var dto = new SimpleTagDto();
            dto.SetSupportedFunctions(ServiceFunctions.None);

            //ATTEMPT
            var status = service.DoAction(null, dto);

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("Running an action is not setup for this data.");
            dto.FunctionsCalledCommaDelimited.ShouldEqual("");

        }

        [TestCase(InstrumentedOpFlags.NormalOperation, true, "CopyDtoToData")]
        [TestCase(InstrumentedOpFlags.ForceActionFail, false, "CopyDtoToData")]
        [TestCase(InstrumentedOpFlags.ForceActionWarnWithWrite, true, "CopyDtoToData")]
        [TestCase(InstrumentedOpFlags.FailOnCopyDtoToData, false, "CopyDtoToData")]
        public void Check02ActionFlowAction(InstrumentedOpFlags errorFlag, bool isValid, string expectedFunctionsCalled)
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var service = new ActionService<IEmptyTestAction, int, Tag, SimpleTagDto>(dummyDb, new EmptyTestAction());
            var dto = new SimpleTagDto(errorFlag);
            if (errorFlag == InstrumentedOpFlags.ForceActionFail)
                dto.TagId = 2;
            else if (errorFlag == InstrumentedOpFlags.ForceActionWarnWithWrite)
                dto.TagId = 1;

            //ATTEMPT
            var status = service.DoAction(null, dto);

            //VERIFY
            status.IsValid.ShouldEqual(isValid, status.Errors);
            status.Result.ShouldEqual(status.IsValid ? dto.TagId : 0);
            dto.FunctionsCalledCommaDelimited.ShouldEqual(expectedFunctionsCalled);
        }

        //[TestCase(InstrumentedOpFlags.NormalOperation, true, "CopyDtoToData")]
        //[TestCase(InstrumentedOpFlags.ForceActionFail, false, "CopyDtoToData,SetupSecondaryData")]
        //[TestCase(InstrumentedOpFlags.ForceActionWarnWithWrite, true, "CopyDtoToData")]
        //[TestCase(InstrumentedOpFlags.ForceActionkWarnNoWrite, true, "CopyDtoToData")]
        //[TestCase(InstrumentedOpFlags.FailOnCopyDtoToData, false, "CopyDtoToData,SetupSecondaryData")]
        //public void Check05ActionFlowDbAction(InstrumentedOpFlags errorFlag, bool isValid, string expectedFunctionsCalled)
        //{
        //    //SETUP
        //    var dummyDb = new DummyIDbContextWithValidation();
        //    var service = new ActionDbService<IEmptyTestAction, Tag, SimpleTagDto>(dummyDb, new EmptyTestAction());
        //    var dto = new SimpleTagDto(errorFlag);
        //    if (errorFlag == InstrumentedOpFlags.ForceActionFail)
        //        dto.TagId = 2;
        //    else if (errorFlag == InstrumentedOpFlags.ForceActionWarnWithWrite || errorFlag == InstrumentedOpFlags.ForceActionkWarnNoWrite)
        //        dto.TagId = 1;

        //    //ATTEMPT
        //    var status = service.DoDbAction(dto);

        //    //VERIFY
        //    status.IsValid.ShouldEqual(isValid);
        //    dto.FunctionsCalledCommaDelimited.ShouldEqual(expectedFunctionsCalled);
        //    dummyDb.SaveChangesWithValidationCalled.ShouldEqual(isValid && errorFlag != InstrumentedOpFlags.ForceActionkWarnNoWrite);
        //}
    }
}
