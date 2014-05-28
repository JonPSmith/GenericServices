using GenericServices;
using GenericServices.Services;
using NUnit.Framework;
using Tests.Actions;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;


namespace Tests.UnitTests.Group08Services
{
    class Test10RunAction
    {

        [Test]
        public void Check01ReferencesOk()
        {

            //SETUP    

            //ATTEMPT
            IActionService<IEmptyTestAction, Tag> taskService = new ActionService<IEmptyTestAction, Tag>(new EmptyTestAction());
            IActionService<IEmptyTestAction, Tag, SimpleTagDto> taskDtoService = 
                new ActionService<IEmptyTestAction, Tag, SimpleTagDto>(null, new EmptyTestAction());

            //VERIFY
            (taskService is ActionService<IEmptyTestAction, Tag>).ShouldEqual(true);
        }

        //--------------------------------------------------------------
        //non dto tasking

        [Test]
        public void Check02RunActionTestActionServiceIsValidOk()
        {

            //SETUP    
            var taskService = new ActionService<IEmptyTestAction, Tag>(new EmptyTestAction());

            //ATTEMPT
            var tag = new Tag { TagId = 0 };      //this controls the task failing. 0 means success
            var status = taskService.DoAction(tag);

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.SuccessMessage.ShouldEqual("Successful");
        }

        [Test]
        public void Check03RunActionEmptyFailOk()
        {

            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var taskService = new ActionDbService<IEmptyTestAction, Tag>(dummyDb, new EmptyTestAction());

            //ATTEMPT
            var tag = new Tag { TagId = 2 };      //this controls the task failing. 0 means success
            var status = taskService.DoDbAction(tag);

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("forced fail");
        }

        [Test]
        public void Check05RunDbActionTestActionServiceIsValidOk()
        {

            //SETUP    
            var dummyDb = new DummyIDbContextWithValidation();
            var taskService = new ActionDbService<IEmptyTestAction, Tag>(dummyDb, new EmptyTestAction());

            //ATTEMPT
            var tag = new Tag { TagId = 0 };      //this controls the task failing. 0 means success
            var status = taskService.DoDbAction(tag);

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.SuccessMessage.ShouldEqual("Successful... and written to database.");
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(true);
        }

        [Test]
        public void Check06RunDbActionEmptyFailOk()
        {

            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var taskService = new ActionDbService<IEmptyTestAction, Tag>(dummyDb, new EmptyTestAction());

            //ATTEMPT
            var tag = new Tag { TagId = 2 };      //this controls the task failing. 0 means success
            var status = taskService.DoDbAction(tag);

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("forced fail");
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
        }

        //-------------------------------------------------------------------------------
        //now dto based tasking

        [Test]
        public void Check10SetupActionOk()
        {

            //SETUP    
            var taskService = new ActionSetupService<Tag, SimpleTagDto>(null);

            //ATTEMPT
            var dto = taskService.GetDto();

            //VERIFY
            dto.ShouldNotEqualNull();
        }

        [Test]
        public void Check12RunDbActionTestActionServiceIsValidOk()
        {

            //SETUP    
            var taskService = new ActionService<IEmptyTestAction, Tag, SimpleTagDto>(null, new EmptyTestAction());

            //ATTEMPT
            var dto = new SimpleTagDto();
            dto.TagId = 0 ;      //this controls the task failing. 0 means success
            var status = taskService.DoAction(dto);

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.SuccessMessage.ShouldEqual("Successful");
        }

        [Test]
        public void Check13RunDbActionEmptyFailOk()
        {

            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var taskService = new ActionDbService<IEmptyTestAction, Tag, SimpleTagDto>(dummyDb, new EmptyTestAction());

            //ATTEMPT
            var dto = new SimpleTagDto();
            dto.TagId = 2 ;      //this controls the task failing. 0 means success
            var status = taskService.DoDbAction(dto);

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("forced fail");
        }

        [Test]
        public void Check15RunDbActionTestActionServiceIsValidOk()
        {

            //SETUP    
            var dummyDb = new DummyIDbContextWithValidation();
            var taskService = new ActionDbService<IEmptyTestAction, Tag, SimpleTagDto>(dummyDb, new EmptyTestAction());

            //ATTEMPT
            var dto = new SimpleTagDto();
            dto.TagId = 0 ;      //this controls the task failing. 0 means success
            var status = taskService.DoDbAction(dto);

            //VERIFY
            status.IsValid.ShouldEqual(true);
            status.SuccessMessage.ShouldEqual("Successful... and written to database.");
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(true);
        }

        [Test]
        public void Check16RunDbActionEmptyFailOk()
        {

            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var taskService = new ActionDbService<IEmptyTestAction, Tag, SimpleTagDto>(dummyDb, new EmptyTestAction());

            //ATTEMPT
            var dto = new SimpleTagDto();
            dto.TagId = 2 ;      //this controls the task failing. 0 means success
            var status = taskService.DoDbAction(dto);

            //VERIFY
            status.IsValid.ShouldEqual(false);
            status.Errors.Count.ShouldEqual(1);
            status.Errors[0].ErrorMessage.ShouldEqual("forced fail");
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
        }

    }
}
