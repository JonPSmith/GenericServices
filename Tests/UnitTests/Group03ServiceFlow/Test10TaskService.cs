using GenericServices.Concrete;
using NUnit.Framework;
using Tests.DataClasses;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;
using Tests.Tasks;

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
            var taskService = new TaskService<ITestTaskTask, Tag, SimpleTagDto>(dummyDb, new TestTaskTask());
            var dto = new SimpleTagDto();
            dto.SetSupportedFunctions(ServiceFunctions.None);

            //ATTEMPT
            var status = taskService.RunTask(dto);

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("Running a task is not setup for this data.");
            dto.FunctionsCalledCommaDelimited.ShouldEqual("SetupSecondaryData");
            
        }

        [TestCase(InstrumentedOpFlags.NormalOperation, true, "CopyDtoToData,CopyDataToDto,SetupSecondaryData")]
        [TestCase(InstrumentedOpFlags.ForceTaskFail, false, "CopyDtoToData,SetupSecondaryData")]
        [TestCase(InstrumentedOpFlags.ForceTaskWarnWithWrite, true, "CopyDtoToData,CopyDataToDto,SetupSecondaryData")]
        [TestCase(InstrumentedOpFlags.FailOnCopyDtoToData, false, "CopyDtoToData,SetupSecondaryData")]
        [TestCase(InstrumentedOpFlags.FailOnCopyDataToDto, false, "CopyDtoToData,CopyDataToDto,SetupSecondaryData")]
        public void Check02TaskFlowTask(InstrumentedOpFlags errorFlag, bool isValid, string expectedFunctionsCalled)
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var service = new TaskService<ITestTaskTask, Tag, SimpleTagDto>(dummyDb, new TestTaskTask());
            var dto = new SimpleTagDto(errorFlag);
            if (errorFlag == InstrumentedOpFlags.ForceTaskFail)
                dto.TagId = 2;
            else if (errorFlag == InstrumentedOpFlags.ForceTaskWarnWithWrite)
                dto.TagId = 1;

            //ATTEMPT
            var status = service.RunTask(dto);

            //VERIFY
            status.IsValid.ShouldEqual(isValid, status.Errors);
            dto.FunctionsCalledCommaDelimited.ShouldEqual(expectedFunctionsCalled);
        }

        [TestCase(InstrumentedOpFlags.NormalOperation, true, "CopyDtoToData")]
        [TestCase(InstrumentedOpFlags.ForceTaskFail, false, "CopyDtoToData,SetupSecondaryData")]
        [TestCase(InstrumentedOpFlags.ForceTaskWarnWithWrite, true, "CopyDtoToData")]
        [TestCase(InstrumentedOpFlags.ForceTaskWarnNoWrite, true, "CopyDtoToData")]
        [TestCase(InstrumentedOpFlags.FailOnCopyDtoToData, false, "CopyDtoToData,SetupSecondaryData")]
        public void Check05TaskFlowDbTask(InstrumentedOpFlags errorFlag, bool isValid, string expectedFunctionsCalled)
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var service = new TaskService<ITestTaskTask, Tag, SimpleTagDto>(dummyDb, new TestTaskTask());
            var dto = new SimpleTagDto(errorFlag);
            if (errorFlag == InstrumentedOpFlags.ForceTaskFail)
                dto.TagId = 2;
            else if (errorFlag == InstrumentedOpFlags.ForceTaskWarnWithWrite || errorFlag == InstrumentedOpFlags.ForceTaskWarnNoWrite)
                dto.TagId = 1;

            //ATTEMPT
            var status = service.RunDbTask(dto);

            //VERIFY
            status.IsValid.ShouldEqual(isValid);
            dto.FunctionsCalledCommaDelimited.ShouldEqual(expectedFunctionsCalled);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(isValid && errorFlag != InstrumentedOpFlags.ForceTaskWarnNoWrite);
        }
    }
}
