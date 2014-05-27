using System;

namespace GenericServices.Actions
{
    public class ProgressWithOptionalMessage
    {

        public int PercentageDone { get; private set; }

        public ProgressMessage OptionalMessage { get; private set; }

        public ProgressWithOptionalMessage(int percentageDone, ProgressMessage optionalMessage)
        {
            PercentageDone = Math.Min(0, Math.Max(100, percentageDone));
            OptionalMessage = optionalMessage;
        }
    }
}
