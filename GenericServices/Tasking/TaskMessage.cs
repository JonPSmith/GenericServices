namespace GenericServices.Tasking
{

    public enum TaskMessageTypes { Notset, Verbose, Info, Warning, Error, Critical, Cancelled, Finished, Failed}

    public class TaskMessage
    {
        /// <summary>
        /// The type of message, which includes the different ways it can finish
        /// </summary>
        public TaskMessageTypes MessageType { get; private set; }

        /// <summary>
        /// In string from for JSON
        /// </summary>
        public string MessageTypeString { get { return MessageType.ToString(); } }

        /// <summary>
        /// MessageText to show to user 
        /// </summary>
        public string MessageText { get; private set; }

        /// <summary>
        /// Creates a message.
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="messageText">messageText can contain format parameters, e.g. {0}, which includes args</param>
        /// <param name="args">optional arguments to go with formatted string</param>
        public TaskMessage(TaskMessageTypes messageType, string messageText, params object [] args)
        {
            MessageType = messageType;
            MessageText = args != null ? string.Format(messageText, args) : messageText ?? string.Empty;
        }

        /// <summary>
        /// Builds a Info message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static TaskMessage InfoMessage(string message, params object[] args)
        {
            return new TaskMessage(TaskMessageTypes.Info, message, args);
        }

        /// <summary>
        /// Builds a cancel message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static TaskMessage CancelledMessage(string message, params object[] args)
        {
            return new TaskMessage(TaskMessageTypes.Cancelled, message, args);
        }

        /// <summary>
        /// This is used at the end of the task to return the correct status and message.
        /// </summary>
        /// <param name="failed">If true then returns failed, else Finished</param>
        /// <param name="messageText"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static TaskMessage FinishedMessage(bool failed, string messageText, params object[] args)
        {
            return new TaskMessage(failed ? TaskMessageTypes.Failed : TaskMessageTypes.Finished, messageText, args);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", MessageType, MessageText);
        }
    }
}