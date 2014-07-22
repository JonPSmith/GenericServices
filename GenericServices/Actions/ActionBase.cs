using System;
using GenericServices.ActionComms;
using GenericServices.Logger;

namespace GenericServices.Actions
{
    public abstract class ActionBase
    {

        private readonly IGenericLogger _logger;

        /// <summary>
        /// Override this to return true if you want the calling service to write to database.
        /// If the method exited with status IsValid and the warning check (see ICheckIfWarings)
        /// passes the it will call SubmitChanges to commit any data to the database
        /// </summary>
        public virtual bool SubmitChangesOnSuccess { get { return false; } }
      
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
