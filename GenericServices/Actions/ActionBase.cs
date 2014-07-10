using System;
using GenericServices.ActionComms;
using GenericServices.Logger;
using GenericServices.Services;

namespace GenericServices.Actions
{
    public abstract class ActionBase
    {

        private readonly IGenericLogger _logger;

        /// <summary>
        /// If true then the caller should call EF SubmitChanges if the method exited with status IsValid and
        /// it looks to see if the data part has a ICheckIfWarnings and if the WriteEvenIfWarning is false
        /// and there are warnings then it does not call SubmitChanges
        /// </summary>
        public abstract bool SubmitChangesOnSuccess { get; }
      
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

        
        //---------------------------------------------------
        //private helpers

        protected void SendtoLogger(ProgressMessage message)
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
