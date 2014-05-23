namespace GenericServices.Tasking
{
    public interface ITaskComms
    {
        /// <summary>
        /// This sends a status update to the user from the running task
        /// </summary>
        /// <param name="percentageDone">goes from 0 to 100</param>
        /// <param name="message">message, with message type in. Can be null for no message</param>
        void ReportProgress(int percentageDone, TaskMessage message = null);

        /// <summary>
        /// This is true if the user has asked for the task to cancel
        /// </summary>
        bool CancellationPending { get; }

    }
}
