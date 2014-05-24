using GenericServices;
using GenericServices.Concrete;
using GenericServices.Tasking;

namespace Tests.Tasks
{
    public interface IEmptyTask : ITaskService<bool>
    {
    }

    public class EmptyTask : TaskCommunicate, IEmptyTask
    {
        public ISuccessOrErrors Task(ITaskComms taskComms, bool taskData)
        {
            var status = new SuccessOrErrors();
            return taskData
                ? status.SetSuccessMessage("Successful")
                : status.AddSingleError("bool was false, so error");
        }
    }
}
