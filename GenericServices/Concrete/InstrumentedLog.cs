namespace GenericServices.Concrete
{
    public enum CallTypes { Point, Start, End }

    public class InstrumentedLog
    {

        public string CallPoint { get; private set; }

        public CallTypes CallType { get; private set; }

        public long ElapsedMilisecondsSoFar { get; private set; }

        public InstrumentedLog(string callPoint, CallTypes callType, long elapsedMilisecondsSoFar)
        {
            CallPoint = callPoint;
            CallType = callType;
            ElapsedMilisecondsSoFar = elapsedMilisecondsSoFar;
        }

        public override string ToString()
        {
            return string.Format("CallPoint: {0}, CallType: {1}, MilisecondsSoFar: {2}", CallPoint, CallType, ElapsedMilisecondsSoFar);
        }
    }
}
