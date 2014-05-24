using GenericServices.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.Helpers;
using Tests.Tasks;
using Tests.TestOnlyDTOs;

namespace Tests.UnitTests.Group03ServiceFlow
{
    class Test10TaskService
    {

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            using (var db = new SampleWebAppDb())
            {
                DataLayerInitialise.InitialiseThis();
                DataLayerInitialise.ResetDatabaseToTestData(db);
            }
        }


        [Test]
        public void Check01TaskFailOk()
        {

            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var taskService = new TaskService<ITestTaskTask, Tag, TestWithErrorsAndTrackingDto>(dummyDb, new TestTaskTask());
            var dto = new TestWithErrorsAndTrackingDto();
            dto.SupportedFunctionsToUse = ServiceFunctions.None;

            //ATTEMPT
            var status = taskService.RunTask(dto);

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("Running a task is not setup for this data.");
            dto.FunctionsCalledCommaDelimited.ShouldEqual("SetupSecondaryData");
            
        }

        [TestCase(TestErrorsFlags.NoError, true, "CopyDtoToData,CopyDataToDto,SetupSecondaryData")]
        [TestCase(TestErrorsFlags.ForceTaskFail, false, "CopyDtoToData,SetupSecondaryData")]
        [TestCase(TestErrorsFlags.ForceTaskWarnWithWrite, true, "CopyDtoToData,CopyDataToDto,SetupSecondaryData")]
        [TestCase(TestErrorsFlags.FailOnCopyDtoToData, false, "CopyDtoToData,SetupSecondaryData")]
        [TestCase(TestErrorsFlags.FailOnCopyDataToDto, false, "CopyDtoToData,CopyDataToDto,SetupSecondaryData")]
        public void Check02TaskFlowTask(TestErrorsFlags errorFlag, bool isValid, string expectedFunctionsCalled)
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var service = new TaskService<ITestTaskTask, Tag, TestWithErrorsAndTrackingDto>(dummyDb, new TestTaskTask());
            var dto = new TestWithErrorsAndTrackingDto(errorFlag);
            if (errorFlag == TestErrorsFlags.ForceTaskFail)
                dto.TagId = 2;
            else if (errorFlag == TestErrorsFlags.ForceTaskWarnWithWrite)
                dto.TagId = 1;

            //ATTEMPT
            var status = service.RunTask(dto);

            //VERIFY
            status.IsValid.ShouldEqual(isValid, status.Errors);
            dto.FunctionsCalledCommaDelimited.ShouldEqual(expectedFunctionsCalled);
        }

        [TestCase(TestErrorsFlags.NoError, true, "CopyDtoToData")]
        [TestCase(TestErrorsFlags.ForceTaskFail, false, "CopyDtoToData,SetupSecondaryData")]
        [TestCase(TestErrorsFlags.ForceTaskWarnWithWrite, true, "CopyDtoToData")]
        [TestCase(TestErrorsFlags.ForceTaskWarnNoWrite, true, "CopyDtoToData")]
        [TestCase(TestErrorsFlags.FailOnCopyDtoToData, false, "CopyDtoToData,SetupSecondaryData")]
        public void Check05TaskFlowDbTask(TestErrorsFlags errorFlag, bool isValid, string expectedFunctionsCalled)
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var service = new TaskService<ITestTaskTask, Tag, TestWithErrorsAndTrackingDto>(dummyDb, new TestTaskTask());
            var dto = new TestWithErrorsAndTrackingDto(errorFlag);
            if (errorFlag == TestErrorsFlags.ForceTaskFail)
                dto.TagId = 2;
            else if (errorFlag == TestErrorsFlags.ForceTaskWarnWithWrite || errorFlag == TestErrorsFlags.ForceTaskWarnNoWrite)
                dto.TagId = 1;

            //ATTEMPT
            var status = service.RunDbTask(dto);

            //VERIFY
            status.IsValid.ShouldEqual(isValid);
            dto.FunctionsCalledCommaDelimited.ShouldEqual(expectedFunctionsCalled);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(isValid && errorFlag != TestErrorsFlags.ForceTaskWarnNoWrite);
        }
    }
}
