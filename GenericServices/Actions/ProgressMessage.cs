namespace GenericServices.Actions
{
    public enum ProgressMessageTypes { Notset, Verbose, Info, Warning, Error, Critical, Cancelled, Finished, Failed }

    public class ProgressMessage
    {
        /// <summary>
        /// The type of message, which includes the different ways it can finish
        /// </summary>
        public ProgressMessageTypes MessageType { get; private set; }

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
        public ProgressMessage(ProgressMessageTypes messageType, string messageText, params object [] args)
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
        public static ProgressMessage InfoMessage(string message, params object[] args)
        {
            return new ProgressMessage(ProgressMessageTypes.Info, message, args);
        }

        /// <summary>
        /// Builds a cancel message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ProgressMessage CancelledMessage(string message, params object[] args)
        {
            return new ProgressMessage(ProgressMessageTypes.Cancelled, message, args);
        }

        /// <summary>
        /// This is used at the end of the task to return the correct status and message.
        /// </summary>
        /// <param name="failed">If true then returns failed, else Finished</param>
        /// <param name="messageText"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ProgressMessage FinishedMessage(bool failed, string messageText, params object[] args)
        {
            return new ProgressMessage(failed ? ProgressMessageTypes.Failed : ProgressMessageTypes.Finished, messageText, args);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", MessageType, MessageText);
        }
    }
}