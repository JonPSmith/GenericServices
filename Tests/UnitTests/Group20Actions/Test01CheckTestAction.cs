using NUnit.Framework;
using Tests.Actions;
using Tests.DataClasses.Concrete;
using Tests.Helpers;

namespace Tests.UnitTests.Group20Actions
{
    class Test01CheckTestAction
    {

        [Test]
        public void Check01RunActionSuccessOk()
        {
            //SETUP  
            var testAction = new EmptyTestAction(false);

            //ATTEMPT
            var data = new Tag {TagId = -123};
            var status = testAction.DoAction(data);

            //VERIFY
            status.IsValid.ShouldEqual(true, status.Errors);
            status.Result.ShouldEqual(data.TagId);
            testAction.DisposeWasCalled.ShouldEqual(false);
        }

        [Test]
        public void Check05CheckDisposeCalledOk()
        {
            //SETUP  
            var testAction = new EmptyTestAction(false);

            //ATTEMPT
            testAction.Dispose();

            //VERIFY
            testAction.DisposeWasCalled.ShouldEqual(true);
        }
        
    }
}
