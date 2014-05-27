using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GenericServices.Concrete;

namespace GenericServices.Actions
{
    public class ActionComms : IActionComms
    {
        private readonly CancellationToken _ctx;
        private readonly IProgress<ProgressWithOptionalMessage> _progressReporter;

        /// <summary>
        /// This is true if the user has asked for the task to cancel
        /// </summary>
        public bool CancellationPending { get { return _ctx.IsCancellationRequested; } }

        /// <summary>
        /// This will throw an OperationCanceledException if the user has asked for the task to be cancelled.
        /// This is the recommended way of checking for cancellation
        /// </summary>
        public void ThrowExceptionIfCancelPending()
        {
            _ctx.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// This sends a status update to the user from the running task
        /// </summary>
        /// <param name="percentageDone">goes from 0 to 100</param>
        /// <param name="message">message, with message type in. Can be null for no message</param>
        public void ReportProgress(int percentageDone, ProgressMessage message = null)
        {
            _progressReporter.Report( new ProgressWithOptionalMessage(percentageDone, message));
        }

        public ActionComms(CancellationToken ctx, IProgress<ProgressWithOptionalMessage> progressReporter)
        {
            _ctx = ctx;
            _progressReporter = progressReporter;
        }
    }
}
