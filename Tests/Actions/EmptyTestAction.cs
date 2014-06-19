using GenericServices;
using GenericServices.Actions;
using GenericServices.Services;
using Tests.DataClasses.Concrete;

namespace Tests.Actions
{
    public interface IEmptyTestAction : IActionDefn<Tag>
    {
    }


    public class EmptyTestAction : ActionBase, IEmptyTestAction
    {

        /// <summary>
        /// This allows the action to configure what it supports, which then affects what the user sees
        /// Note: it must be a constant as it is read just after the action is created
        /// </summary>
        public override ActionFlags ActionConfig
        {
            get { return ActionFlags.NoProgressSent | ActionFlags.NoMessagesSent | ActionFlags.CancelNotSupported; }
        }

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