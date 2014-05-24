namespace GenericServices.Concrete
{
    public interface IDataTaskService<TTask, TTaskData>
        where TTask : class, ITaskService<TTaskData> 
    {
    }
}