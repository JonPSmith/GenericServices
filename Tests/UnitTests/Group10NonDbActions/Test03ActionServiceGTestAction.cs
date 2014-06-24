using System.Linq;
using GenericServices;
using GenericServices.Services;
using NUnit.Framework;
using Tests.Actions;
using Tests.Helpers;

namespace Tests.UnitTests.Group10NonDbActions
{
    class Test03ActionServiceGTestAction
    {

        [Test]
        public void Check01ReferencesOk()
        {

            //SETUP    

            //ATTEMPT
            IActionService<GTestAction, int, GTestActionData> actionService = new ActionService<GTestAction, int, GTestActionData>(null, new GTestAction());
            IActionService<GTestAction, int, GTestActionData, GTestActionDto> actionDtoService =
                new ActionService<GTestAction, int, GTestActionData, GTestActionDto>(null, new GTestAction());

            //VERIFY
            (actionService is ActionService<GTestAction, int, GTestActionData>).ShouldEqual(true);
        }

        [Test]
        public void Check02RunActionServiceSuccessOk()
        {
            //SETUP
            var dummyDb = new DummyIDbContextWithValidation();
            var testAction = new GTestAction();
            var service = new ActionService<GTestAction, int, GTestActionData>(dummyDb, testAction);

            //ATTEMPT
            var data = new GTestActionData();
            var status = service.DoAction(null, data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.ShouldEqual(data.NumIterations);
            testAction.DisposeWasCalled.ShouldEqual(true);
        }

        [Test]
        public void Check03RunActionServiceCommsOk()
        {
            //SETUP  
            var dummyDb = new DummyIDbContextWithValidation();
            var mockComms = new MockActionComms();
            var service = new ActionService<GTestAction, int, GTestActionData>(dummyDb, new GTestAction());

            //ATTEMPT
            var data = new GTestActionData();
            var status = service.DoAction(mockComms, data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            CollectionAssert.AreEqual(new []{0, 20, 40, 60, 80, 100}, mockComms.AllProgressReports.Select(x => x.PercentageDone));
        }

        [Test]
        public void Check10RunActionDtoSuccessOk()
        {
            //SETUP  
            var dummyDb = new DummyIDbContextWithValidation();
            var mockComms = new MockActionComms();
            var testAction = new GTestAction();
            var service = new ActionService<GTestAction, int, GTestActionData, GTestActionDto>(dummyDb, testAction);

            //ATTEMPT
            var dto = new GTestActionDto
            {
                NumIterations = 3
            };
            var status = service.DoAction(mockComms, dto);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.ShouldEqual(dto.NumIterations);
            testAction.DisposeWasCalled.ShouldEqual(true);
            dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
        }

        [Test]
        public void Check11RunActionDtoCommsOk()
        {
            //SETUP  
            var dummyDb = new DummyIDbContextWithValidation();
            var mockComms = new MockActionComms();
            var testAction = new GTestAction();
            var service = new ActionService<GTestAction, int, GTestActionData, GTestActionDto>(dummyDb, testAction);

            //ATTEMPT
            var dto = new GTestActionDto
            {
                NumIterations = 3
            };
            var status = service.DoAction(mockComms, dto);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            CollectionAssert.AreEqual(new[] { 0, 33, 66, 100 }, mockComms.AllProgressReports.Select(x => x.PercentageDone));
        }
        
    }
}
