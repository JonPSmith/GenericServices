using GenericServices.Concrete;
using NUnit.Framework;
using Tests.Helpers;
using Tests.Tasks;

namespace Tests.UnitTests.Group04Services
{
    class Test10RunTask
    {

        [Test]
        public void Check01ReferencesOk()
        {

            //SETUP    

            //ATTEMPT
            IDataTaskService<IEmptyTask, bool> taskService = new TaskService<IEmptyTask, bool>(null, new EmptyTask());

            //VERIFY
            (taskService is TaskService<IEmptyTask, bool>).ShouldEqual(true);
        }

        [Test]
        public void Check02RunTaskEmptyTaskIsValidOk()
        {

            //SETUP    
            var taskService = new TaskService<IEmptyTask, bool>(null, new EmptyTask());

            //ATTEMPT
            var status = taskService.RunTask(true);

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.SuccessMessage.ShouldEqual("Successful");
        }

        [Test]
        public void Check03RunTaskEmptyFailOk()
        {

            //SETUP
            var taskService = new TaskService<IEmptyTask, bool>(null, new EmptyTask());

            //ATTEMPT
            var status = taskService.RunDbTask(false);

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("bool was false, so error");
        }

        [Test]
        public void Check05RunDbTaskEmptyTaskIsValidOk()
        {

            //SETUP    
            var dummyDb = new DummyIDbContextWithValidation();
            var taskService = new TaskService<IEmptyTask, bool>(dummyDb, new EmptyTask());

            //ATTEMPT
            var status = taskService.RunDbTask(true);

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
            var taskService = new TaskService<IEmptyTask, bool>(dummyDb, new EmptyTask());

            //ATTEMPT
            var status = taskService.RunDbTask(false);

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("bool was false, so error");
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
        }

    }
}
