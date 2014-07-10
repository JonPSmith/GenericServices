using System;
using GenericServices.Actions;

namespace GenericServices.ActionComms
{
    public abstract class ActionCommsBase : ActionBase
    {

        private int _lastReportedProgressPercentage = -1;


        /// <summary>
        /// This allows the action to configure what it supports, which then affects what the user sees
        /// Note: it must be a constant as it is read just after the action is created
        /// </summary>
        public abstract ActionFlags ActionConfig { get; }

        /// <summary>
        /// This reports progress with optional message.
        /// After sending the message it checks for cancellation and throws OperationCanceledException if pending
        /// </summary>
        /// <param name="actionComms">The comms channel for handling progress and cancellation</param>
        /// <param name="percentageDone">must be between 0 and 100</param>
        /// <param name="message">optional message to show user</param>
        protected void ReportProgressAndCheckCancel(IActionComms actionComms, int percentageDone, ProgressMessage message = null)
        {
            if (actionComms == null) return;

            ReportProgress(actionComms, percentageDone, message);
            actionComms.ThrowExceptionIfCancelPending();
        }

        /// <summary>
        /// This reports progress with optional message.
        /// </summary>
        /// <param name="actionComms">The comms channel for handling progress and cancellation</param>
        /// <param name="percentageDone">must be between 0 and 100</param>
        /// <param name="message">optional message to show user</param>
        protected void ReportProgress(IActionComms actionComms, int percentageDone, ProgressMessage message = null)
        {
            if (actionComms == null) return;

            var percentageToReport = (int)(LowerBound + ((Math.Min(percentageDone, 100) / 100.0)) * (UpperBound - LowerBound));
            if (percentageToReport != _lastReportedProgressPercentage || message != null)
                //we only report progess if the progress percent has changed or there is a message to send
                actionComms.ReportProgress(percentageToReport, message);

            if (message != null)
                SendtoLogger(message);

            _lastReportedProgressPercentage = percentageToReport;
        }

        /// <summary>
        /// This will throw an OperationCanceledException if the user has asked for the task to be cancelled.
        /// This is the recommended way of checking for cancellation
        /// <param name="actionComms">The comms channel for handling progress and cancellation</param>
        /// </summary>
        protected void ThrowExceptionIfCancelPending(IActionComms actionComms)
        {
            if (actionComms != null)
                actionComms.ThrowExceptionIfCancelPending();
        }

        /// <summary>
        /// Returns true if user has asked for cancellation
        /// </summary>
        /// <param name="actionComms">The comms channel for handling progress and cancellation</param>
        /// <returns>Returns true if user has asked for cancellation</returns>
        protected bool CancelPending(IActionComms actionComms)
        {
            return actionComms != null && actionComms.CancellationPending;
        }

    }
}
