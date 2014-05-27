using GenericServices.Actions;
using GenericServices.Services;

namespace GenericServices
{
    public interface IActionComms
    {

        /// <summary>
        /// This is true if the user has asked for the task to cancel
        /// </summary>
        bool CancellationPending { get; }

        /// <summary>
        /// This will throw an OperationCanceledException if the user has asked for the task to be cancelled.
        /// This is the recommended way of checking for cancellation
        /// </summary>
        void ThrowExceptionIfCancelPending();

        /// <summary>
        /// This sends a status update to the user from the running task
        /// </summary>
        /// <param name="percentageDone">goes from 0 to 100</param>
        /// <param name="message">message, with message type in. Can be null for no message</param>
        void ReportProgress(int percentageDone, ProgressMessage message = null);






    }
}
