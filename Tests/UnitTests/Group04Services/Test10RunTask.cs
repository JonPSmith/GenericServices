using GenericServices.Concrete;
using NUnit.Framework;
using Tests.DataClasses.Concrete;
using Tests.Helpers;
using Tests.Tasks;
using Tests.TestOnlyDTOs;

namespace Tests.UnitTests.Group04Services
{
    class Test10RunTask
    {

        [Test]
        public void Check01ReferencesOk()
        {

            //SETUP    

            //ATTEMPT
            ITaskService<ITestTaskTask, Tag> taskService = new TaskService<ITestTaskTask, Tag>(null, new TestTaskTask());
            ITaskService<ITestTaskTask, Tag, TestWithErrorsAndTrackingDto> taskDtoService = 
                new TaskService<ITestTaskTask, Tag, TestWithErrorsAndTrackingDto>(null, new TestTaskTask());

            //VERIFY
            (taskService is TaskService<ITestTaskTask, Tag>).ShouldEqual(true);
        }

        //--------------------------------------------------------------
        //non dto tasking

        [Test]
        public void Check02RunTaskEmptyTaskIsValidOk()
        {

            //SETUP    
            var taskService = new TaskService<ITestTaskTask, Tag>(null, new TestTaskTask());

            //ATTEMPT
            var tag = new Tag { TagId = 0 };      //this controls the task failing. 0 means success
            var status = taskService.RunTask(tag);

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.SuccessMessage.ShouldEqual("Successful");
        }

        [Test]
        public void Check03RunTaskEmptyFailOk()
        {

            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var taskService = new TaskService<ITestTaskTask, Tag>(dummyDb, new TestTaskTask());

            //ATTEMPT
            var tag = new Tag { TagId = 2 };      //this controls the task failing. 0 means success
            var status = taskService.RunDbTask(tag);

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("forced fail");
        }

        [Test]
        public void Check05RunDbTaskEmptyTaskIsValidOk()
        {

            //SETUP    
            var dummyDb = new DummyIDbContextWithValidation();
            var taskService = new TaskService<ITestTaskTask, Tag>(dummyDb, new TestTaskTask());

            //ATTEMPT
            var tag = new Tag { TagId = 0 };      //this controls the task failing. 0 means success
            var status = taskService.RunDbTask(tag);

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.SuccessMessage.ShouldEqual("Successful... and written to database");
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(true);
        }

        [Test]
        public void Check06RunDbTaskEmptyFailOk()
        {

            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var taskService = new TaskService<ITestTaskTask, Tag>(dummyDb, new TestTaskTask());

            //ATTEMPT
            var tag = new Tag { TagId = 2 };      //this controls the task failing. 0 means success
            var status = taskService.RunDbTask(tag);

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("forced fail");
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
        }

        //-------------------------------------------------------------------------------
        //now dto based tasking


    }
}
