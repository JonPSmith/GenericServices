using GenericServices;
using GenericServices.Concrete;
using Tests.DataClasses.Concrete;

namespace Tests.Actions
{
    public interface ITestActionService : IActionDefn<Tag>
    {
    }


    public class TestActionService : ActionCommunicate, ITestActionService
    {

        public ISuccessOrErrors DoAction(IActionComms actionComms, Tag actionData)
        {
            var status = new SuccessOrErrors();

            //we use the TagId for testing
            //0 means success
            //1 means success, but with warning
            //2 and above mean fail

            if (actionData.TagId == 1)
                status.AddWarning("This is a warning message");

            return actionData.TagId <= 1
                ? status.SetSuccessMessage("Successful")
                : status.AddSingleError("forced fail");
        }
    }
}