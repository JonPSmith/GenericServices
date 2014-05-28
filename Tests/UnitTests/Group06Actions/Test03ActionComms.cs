using GenericServices.Services;
using NUnit.Framework;
using Tests.Actions;
using Tests.DataClasses.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group06Actions
{
    class Test03ActionComms
    {

        [Test]
        public void Check01RunActionSuccessOk()
        {
            //SETUP  
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new CommsTestAction();
            var service = new ActionService<ICommsTestAction, CommsTestActionData>(testAction);

            //ATTEMPT
            var data = new CommsTestActionData();
            var status = service.DoAction(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            testAction.DisposeWasCalled.ShouldEqual(true);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
        }


        [Test]
        public void Check02RunDbActionSuccessOk()
        {
            //SETUP  
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new CommsTestAction();
            var service = new ActionDbService<ICommsTestAction, CommsTestActionData>(dummyDb, testAction);

            //ATTEMPT
            var data = new CommsTestActionData();
            var status = service.DoDbAction(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            testAction.DisposeWasCalled.ShouldEqual(true);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(true);
        }

        [Test]
        public void Check03RunActionDtoSuccessOk()
        {
            //SETUP  
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new CommsTestAction();
            var service = new ActionService<ICommsTestAction, CommsTestActionData, CommsTestActionDto>(dummyDb, testAction);

            //ATTEMPT
            var dto = new CommsTestActionDto();
            var status = service.DoAction(dto);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            testAction.DisposeWasCalled.ShouldEqual(true);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
        }


        [Test]
        public void Check04RunDbActionDtoSuccessOk()
        {
            //SETUP  
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new CommsTestAction();
            var service = new ActionDbService<ICommsTestAction, CommsTestActionData, CommsTestActionDto>(dummyDb, testAction);

            //ATTEMPT
            var dto = new CommsTestActionDto();
            var status = service.DoDbAction(dto);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            testAction.DisposeWasCalled.ShouldEqual(true);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(true);
        }

        //----------
        //no dispose

        [Test]
        public void Check05RunActionSuccessNoDisposeOk()
        {
            //SETUP  
            var testAction = new EmptyTestAction();
            var service = new ActionService<IEmptyTestAction, Tag>(testAction);

            //ATTEMPT
            var data = new Tag();
            var status = service.DoAction(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
        }


        [Test]
        public void Check05RunDbActionSuccessNoDisposeOk()
        {
            //SETUP  
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new EmptyTestAction();
            var service = new ActionDbService<IEmptyTestAction, Tag>(dummyDb, testAction);

            //ATTEMPT
            var data = new Tag();
            var status = service.DoDbAction(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(true);
        }

        
        //--------------------------------------------------------------------------

        [Test]
        public void Check10CheckMessagesOk()
        {
            //SETUP  
            var testAction = new CommsTestAction();
            var service = new ActionService<ICommsTestAction, CommsTestActionData>(testAction);

            //ATTEMPT
            var data = new CommsTestActionData();
            var status = service.DoAction(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            
        }
    }
}
