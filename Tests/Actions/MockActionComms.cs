using System;
using System.Collections.ObjectModel;
using System.Linq;
using GenericServices;
using GenericServices.Actions;

namespace Tests.Actions
{
    class MockActionComms : IActionComms
    {

        private readonly Collection<ProgressWithOptionalMessage> _messages = new Collection<ProgressWithOptionalMessage>();

        /// <summary>
        /// List of all progress reports with optional messages
        /// </summary>
        public ReadOnlyCollection<ProgressWithOptionalMessage> AllProgressReports { get { return new ReadOnlyCollection<ProgressWithOptionalMessage>(_messages);}}

        /// <summary>
        /// List of progress reports with a message
        /// </summary>
        public ReadOnlyCollection<ProgressWithOptionalMessage> ProgressReportsWithMessage { get
        {
            return new ReadOnlyCollection<ProgressWithOptionalMessage>(_messages.Where(x => x.OptionalMessage != null).ToList());
        } } 


        /// <summary>
        /// 
        /// </summary>
        public bool CancellationPending { get; set; }
        
        
        public void ThrowExceptionIfCancelPending()
        {
            if (CancellationPending)
                throw new OperationCanceledException();
        }

        public void ReportProgress(int percentageDone, ProgressMessage message = null)
        {
            _messages.Add( new ProgressWithOptionalMessage(percentageDone, message));
        }


    }
}
