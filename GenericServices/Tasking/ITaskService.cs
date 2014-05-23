namespace GenericServices.Tasking
{
    public interface ITaskService<in T> where T : class
    {
        /// <summary>
        /// This controls the lower value sent back to reportProgress
        /// Lower and Upper bound are there to allow outer tasks to call inner tasks 
        /// to do part of the work and still report progress properly
        /// </summary>
        int LowerBound { get; set; }

        /// <summary>
        /// This controls the upper bound of the value sent back to reportProgress
        /// </summary>
        int UpperBound { get; set; }

        /// <summary>
        /// This is a general form of a task that can be run sync or async
        /// </summary>
        /// <param name="taskComms">Task communication channel, can be null</param>
        /// <param name="dto">data transfer object holding the setup data needed by the service</param>
        /// <returns></returns>
        ISuccessOrErrors Task(ITaskComms taskComms, T dto);

    }
}
