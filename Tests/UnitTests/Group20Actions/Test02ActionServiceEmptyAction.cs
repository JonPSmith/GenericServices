using GenericServices;
using GenericServices.Services;
using NUnit.Framework;
using Tests.Actions;
using Tests.DataClasses.Concrete;
using Tests.DTOs.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group20Actions
{
    class Test02ActionServiceEmptyAction
    {

        [Test]
        public void Check01ReferencesOk()
        {

            //SETUP    

            //ATTEMPT
            IActionService<int, Tag> actionService = new ActionService<int, Tag>(null, new EmptyTestAction(false));
            IActionService<int, Tag, SimpleTagDto> actionDtoService =
                new ActionService<int, Tag, SimpleTagDto>(null, new EmptyTestAction(false));

            //VERIFY
            (actionService is ActionService<int, Tag>).ShouldEqual(true);
        }

        [Test]
        public void Check02RunActionDisposedCalledOk()
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestAction(false);
            var service = new ActionService<int, Tag>(dummyDb, testAction);

            //ATTEMPT
            var data = new Tag
            {
                TagId = -456
            };
            var status = service.DoAction(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            testAction.DisposeWasCalled.ShouldEqual(true);
        }

        [Test]
        public void Check03RunActionServiceSuccessOk()
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestAction(false);
            var service = new ActionService<int, Tag>(dummyDb, testAction);

            //ATTEMPT
            var data = new Tag
            {
                TagId = -456
            };
            var status = service.DoAction(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.ShouldEqual(-456);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
        }

        [Test]
        public void Check04RunActionServiceCallSubmitOk()
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestAction(true);
            var service = new ActionService<int, Tag>(dummyDb, testAction);

            //ATTEMPT
            var data = new Tag();
            var status = service.DoAction(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.HasWarnings.ShouldEqual(false);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(true);
            status.SuccessMessage.ShouldEndWith("... and written to database.");
        }

        [Test]
        public void Check05RunActionServiceWarningsButSubmitOk()
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestAction(true);
            var service = new ActionService<int, Tag>(dummyDb, testAction);

            //ATTEMPT
            var data = new Tag(true)
            {
                TagId = 1 //means a warning
            };
            var status = service.DoAction(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.HasWarnings.ShouldEqual(true);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(true);
            status.SuccessMessage.ShouldEndWith("... and written to database. (has 1 warnings)");
        }

        [Test]
        public void Check06RunActionServiceWarningsNoSubmitOk()
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestAction(true);
            var service = new ActionService<int, Tag>(dummyDb, testAction);

            //ATTEMPT
            var data = new Tag()
            {
                TagId = 1 //means a warning
            };
            var status = service.DoAction(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.HasWarnings.ShouldEqual(true);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
            status.SuccessMessage.ShouldEndWith("... but NOT written to database as warnings. (has 1 warnings)");
        }

        [Test]
        public void Check07RunActionServiceFailNoSubmitOk()
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestAction(true);
            var service = new ActionService<int, Tag>(dummyDb, testAction);

            //ATTEMPT
            var data = new Tag
            {
                TagId = 3 //will fail
            };
            var status = service.DoAction(data);

            //VERIFY
            status.IsValid.ShouldEqual(false, status.Errors);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
        }

        //----------------------------------------
        //now the dto versions

        [Test]
        public void Check10RunActionDtoSuccessOk()
        {
            //SETUP  
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestAction(false);
            var service = new ActionService<int, Tag, SimpleTagDto>(dummyDb, testAction);

            //ATTEMPT
            var dto = new SimpleTagDto
            {
                TagId = -123,
                Name = "test", 
                Slug = "test"
            };
            var status = service.DoAction(dto);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.ShouldEqual(-123);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
        }

        [Test]
        public void Check11RunActionDtoSubmitOk()
        {
            //SETUP  
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestAction(true);
            var service = new ActionService<int, Tag, SimpleTagDto>(dummyDb, testAction);

            //ATTEMPT
            var dto = new SimpleTagDto
            {
                TagId = -123,
                Name = "test",
                Slug = "test"
            };
            var status = service.DoAction(dto);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.ShouldEqual(-123);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(true);
            status.SuccessMessage.ShouldEndWith("... and written to database.");
        }


        [Test] 
        public void Check12RunActionDtoFailNoSubmitOk()
        {
            //SETUP  
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestAction(true);
            var service = new ActionService<int, Tag, SimpleTagDto>(dummyDb, testAction);

            //ATTEMPT
            var dto = new SimpleTagDto
            {
                TagId = 3,           //will fail,
                Name = "test", 
                Slug = "test"
            };
            var status = service.DoAction(dto);

            //VERIFY
            status.IsValid.ShouldEqual(false, status.Errors);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
        }


    }
}
