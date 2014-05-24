using GenericServices;
using GenericServices.Concrete;
using GenericServices.Tasking;
using Tests.DataClasses.Concrete;

namespace Tests.Tasks
{
    public interface ITestTaskTask : ITaskService<Tag>
    {
    }


    public class TestTaskTask : TaskCommunicate, ITestTaskTask
    {
        public ISuccessOrErrors Task(ITaskComms taskComms, Tag taskData)
        {
            var status = new SuccessOrErrors();

            //we use the TagId for testing
            //0 means success
            //1 means success, but with warning
            //2 and above mean fail

            if (taskData.TagId == 1)
                status.AddWarning("This is a warning message");

            return taskData.TagId <= 1 
                ? status.SetSuccessMessage("Successful") 
                : status.AddSingleError("forced fail");
        }
    }
}