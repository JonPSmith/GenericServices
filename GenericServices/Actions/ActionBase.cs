using System;
using GenericServices.Logger;
using GenericServices.Services;

namespace GenericServices.Actions
{
    public abstract class ActionBase
    {

        private readonly IGenericLogger _logger;

        private int _lastReportedProgressPercentage = -1;


        /// <summary>
        /// If true then the caller should call EF SubmitChanges if the method exited with status IsValid and
        /// it looks to see if the data part has a ICheckIfWarnings and if the WriteEvenIfWarning is false
        /// and there are warnings then it does not call SubmitChanges
        /// </summary>
        public abstract bool SubmitChangesOnSuccess { get; }

        /// <summary>
        /// This allows the action to configure what it supports, which then affects what the user sees
        /// Note: it must be a constant as it is read just after the action is created
        /// </summary>
        public abstract ActionFlags ActionConfig { get; }
      
        /// <summary>
        /// This controls the lower value sent back to reportProgress
        /// Lower and Upper bound are there to allow outer tasks to call inner tasks 
        /// to do part of the work and still report progress properly
        /// </summary>
        public int LowerBound { get; set; }

        /// <summary>
        /// This controls the upper bound of the value sent back to reportProgress
        /// </summary>
        public int UpperBound { get; set; }

        protected ActionBase()
        {
            LowerBound = 0;
            UpperBound = 100;
            _logger = GenericLoggerFactory.GetLogger(GetType().Name);       //give it the name of the inherited type
        }

        /// <summary>
        /// This reports progress with optional message.
        /// After sending the message it checks for cancellation and throws OperationCanceledException if pending
        /// </summary>
        /// <param name="actionComms">The comms channel for handling progress and cancellation</param>
        /// <param name="percentageDone">must be between 0 and 100</param>
        /// <param name="message">optional message to show user</param>
        protected void ReportProgressAndThrowExceptionIfCancelPending(IActionComms actionComms, int percentageDone, ProgressMessage message = null)
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

        //---------------------------------------------------
        //private helpers

        private void SendtoLogger(ProgressMessage message)
        {

            switch (message.MessageType)
            {
                case ProgressMessageTypes.Notset:
                    break;
                case ProgressMessageTypes.Verbose:
                    _logger.Verbose(message.MessageText);
                    break;
                case ProgressMessageTypes.Info:
                    _logger.Info(message.MessageText);
                    break;
                case ProgressMessageTypes.Warning:
                    _logger.Warn(message.MessageText);
                    break;
                case ProgressMessageTypes.Error:
                    _logger.Error(message.MessageText);
                    break;
                case ProgressMessageTypes.Critical:
                    _logger.Critical(message.MessageText);
                    break;
                case ProgressMessageTypes.Finished:
                    _logger.InfoFormat("Finished: {0}", message.MessageText);
                    break;
                case ProgressMessageTypes.Cancelled:
                    _logger.InfoFormat("Cancelled: {0}", message.MessageText);
                    break;
                case ProgressMessageTypes.Failed:
                    _logger.InfoFormat("FAILED: {0}", message.MessageText);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}
