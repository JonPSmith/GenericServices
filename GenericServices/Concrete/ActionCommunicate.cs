using System;

namespace GenericServices.Concrete
{
    public abstract class ActionCommunicate
    {

        //private static log4net.ILog Logger;

        private int _lastReportedProgressPercentage = -1;      
        
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

        protected ActionCommunicate()
        {
            LowerBound = 0;
            UpperBound = 100;
            //Logger = log4net.LogManager.GetLogger(GetType().Name);          //give it the name of the inherited type
        }

        /// <summary>
        /// This reports progress with optional message. Also returns state of cancelPending flag
        /// </summary>
        /// <param name="actionComms">action communication channel, can be null</param>
        /// <param name="percentageDone">must be between 0 and 100</param>
        /// <param name="message">optional message to show user</param>
        /// <returns>Returns true if user has asked for cancellation</returns>
        protected bool ReportProgressAndCheckCancelPending(IActionComms actionComms, int percentageDone, ProgressMessage message = null)
        {
            if (actionComms != null)
            {
                int percentageToReport = (int) (LowerBound + ((Math.Min(percentageDone,100)/100.0))*(UpperBound - LowerBound));
                if (percentageToReport != _lastReportedProgressPercentage || message != null)
                    //we only report progess if the progress percent has changed or there is a message to send
                    actionComms.ReportProgress(percentageToReport, message);

                if (message != null)
                    SendtoLogger(message);

                _lastReportedProgressPercentage = percentageToReport;
                return actionComms.CancellationPending;
            }

            return false;
        }


        /// <summary>
        /// This reports progress with optional message. Also returns state of cancelPending flag
        /// </summary>
        /// <param name="actionComms">Action communication channel, can be null</param>
        /// <param name="percentageDone">must be between 0 and 100</param>
        /// <param name="message">optional message to show user</param>
        /// <returns>Returns true if user has asked for cancellation</returns>
        protected bool ReportProgressAndCheckCancelPending(IActionComms actionComms, double percentageDone,
                                                           ProgressMessage message = null)
        {
            return ReportProgressAndCheckCancelPending(actionComms, (int) Math.Max(percentageDone,0), message);
        }

        /// <summary>
        /// Returns true if user has asked for cancellation
        /// </summary>
        /// <param name="taskComms">Action communication channel, can be null</param>
        /// <returns>Returns true if user has asked for cancellation</returns>
        protected bool CancelPending(IActionComms taskComms)
        {
            return taskComms != null && taskComms.CancellationPending;
        }

        //---------------------------------------------------
        //private helpers

        private static void SendtoLogger(ProgressMessage message)
        {

            switch (message.MessageType)
            {
                //case ProgressMessageTypes.Notset:
                //    break;
                //case ProgressMessageTypes.Verbose:
                //    Logger.Debug(message.MessageText);
                //    break;
                //case ProgressMessageTypes.Info:
                //    Logger.Info(message.MessageText);
                //    break;
                //case ProgressMessageTypes.Warning:
                //    Logger.Warn(message.MessageText);
                //    break;
                //case ProgressMessageTypes.Error:
                //    Logger.Error(message.MessageText);
                //    break;
                //case ProgressMessageTypes.Critical:
                //    Logger.Fatal(message.MessageText);
                //    break;
                //case ProgressMessageTypes.Finished:
                //    Logger.InfoFormat("Finished: {0}", message.MessageText);
                //    break;
                //case ProgressMessageTypes.Cancelled:
                //    Logger.InfoFormat("Cancelled: {0}", message.MessageText);
                //    break;
                //case ProgressMessageTypes.Failed:
                //    Logger.InfoFormat("FAILED: {0}", message.MessageText);
                //    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}
