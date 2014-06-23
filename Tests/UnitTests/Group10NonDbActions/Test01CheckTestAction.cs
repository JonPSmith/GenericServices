using System.Linq;
using GenericServices;
using NUnit.Framework;
using Tests.Actions;
using Tests.Helpers;

namespace Tests.UnitTests.Group10NonDbActions
{
    class Test01CheckTestAction
    {

        [Test]
        public void Check01CommsTestActionConfigOk()
        {
            //SETUP  

            //ATTEMPT
            var testAction = new GTestAction();

            //VERIFY
            testAction.ActionConfig.ShouldEqual(ActionFlags.Normal);
        }

        [Test]
        public void Check02RunActionSuccessOk()
        {
            //SETUP  
            var testAction = new GTestAction();

            //ATTEMPT
            var data = new GTestActionData();
            var status = testAction.DoAction(null, data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.ShouldEqual(data.NumIterations);
        }

        [Test]
        public void Check03RunActionCommsOk()
        {
            //SETUP  
            var mockComms = new MockActionComms();
            var testAction = new GTestAction();

            //ATTEMPT
            var data = new GTestActionData();
            var status = testAction.DoAction(mockComms, data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            CollectionAssert.AreEqual(new []{0, 20, 40, 60, 80, 100}, mockComms.AllProgressReports.Select(x => x.PercentageDone));
        }

        //[Test]
        //public void Check02RunDbActionSuccessOk()
        //{
        //    //SETUP  
        //    var dummyDb = new DummyIDbContextWithValidation();
        //    var testAction = new CommsTestAction();
        //    var service = new ActionDbService<ICommsTestAction, CommsTestActionData>(dummyDb, testAction);

        //    //ATTEMPT
        //    var data = new CommsTestActionData();
        //    var status = service.DoDbAction(data);

        //    //VERIFY
        //    status.IsValid.ShouldEqual(true, status.Errors);
        //    testAction.DisposeWasCalled.ShouldEqual(true);
        //    dummyDb.SaveChangesWithValidationCalled.ShouldEqual(true);
        //}

        //[Test]
        //public void Check10RunActionDtoSuccessOk()
        //{
        //    //SETUP  
        //    var dummyDb = new DummyIDbContextWithValidation();
        //    var mockComms = new MockActionComms();
        //    var testAction = new TestAction();
        //    var service = new ActionService<TestAction, int, CommsTestActionData, CommsTestActionDto>(dummyDb, testAction);

        //    //ATTEMPT
        //    var dto = new CommsTestActionDto
        //    {
        //        NumIterations = 3
        //    };
        //    var status = service.DoAction(mockComms, dto);

        //    //VERIFY
        //    status.IsValid.ShouldEqual(true, status.Errors);
        //    status.Result.ShouldEqual(dto.NumIterations);
        //    testAction.DisposeWasCalled.ShouldEqual(true);
        //    dummyDb.SaveChangesWithValidationCalled.ShouldEqual(false);
        //}

        //[Test]
        //public void Check11RunActionDtoCommsOk()
        //{
        //    //SETUP  
        //    var dummyDb = new DummyIDbContextWithValidation();
        //    var mockComms = new MockActionComms();
        //    var testAction = new TestAction();
        //    var service = new ActionService<TestAction, int, CommsTestActionData, CommsTestActionDto>(dummyDb, testAction);

        //    //ATTEMPT
        //    var dto = new CommsTestActionDto
        //    {
        //        NumIterations = 3
        //    };
        //    var status = service.DoAction(mockComms, dto);

        //    //VERIFY
        //    status.IsValid.ShouldEqual(true, status.Errors);
        //    CollectionAssert.AreEqual(new[] { 0, 33, 66, 100}, mockComms.AllProgressReports.Select(x => x.PercentageDone));
        //}


        //[Test]
        //public void Check04RunDbActionDtoSuccessOk()
        //{
        //    //SETUP  
        //    var dummyDb = new DummyIDbContextWithValidation();
        //    var testAction = new CommsTestAction();
        //    var service = new ActionDbService<ICommsTestAction, CommsTestActionData, CommsTestActionDto>(dummyDb, testAction);

        //    //ATTEMPT
        //    var dto = new CommsTestActionDto();
        //    var status = service.DoDbAction(dto);

        //    //VERIFY
        //    status.IsValid.ShouldEqual(true, status.Errors);
        //    testAction.DisposeWasCalled.ShouldEqual(true);
        //    dummyDb.SaveChangesWithValidationCalled.ShouldEqual(true);
        //}
        
    }
}
