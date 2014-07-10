namespace GenericServices.ActionComms
{
    public class ProgressWithOptionalMessage
    {

        public int PercentageDone { get; private set; }

        public ProgressMessage OptionalMessage { get; private set; }

        public ProgressWithOptionalMessage(int percentageDone, ProgressMessage optionalMessage)
        {
            PercentageDone = percentageDone;
            OptionalMessage = optionalMessage;
        }

        public override string ToString()
        {
            return string.Format("PercentageDone: {0}, OptionalMessage: {1}", PercentageDone, OptionalMessage);
        }
    }
}
