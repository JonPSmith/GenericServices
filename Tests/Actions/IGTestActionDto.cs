using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using GenericServices.Services;

namespace Tests.Actions
{
    public interface IGTestActionDto
    {
        [UIHint("Enum")]
        TestServiceModes Mode { get; set; }

        bool FailToRespondToCancel { get; set; }
        int NumErrorsToExitWith { get; set; }

        [Range(0, 100)]
        double SecondsBetweenIterations { get; set; }

        int NumIterations { get; set; }
        double SecondsDelayToRespondingToCancel { get; set; }

        /// <summary>
        /// Instrumentation property which returns a list of call points as a comma delimited string.
        /// For start/end call points it only returns one entry.
        /// </summary>
        string FunctionsCalledCommaDelimited { get; }

        /// <summary>
        /// Instrumentation property which returns a list of instrumented call points with the time since the dto was created
        /// </summary>
        ReadOnlyCollection<InstrumentedLog> LogOfCalls { get; }

        /// <summary>
        /// This allows the user to control whether data should still be written even if warnings found
        /// </summary>
        bool WriteEvenIfWarning { get; }

        /// <summary>
        /// Instrumentation method which allows a specific point to be logged with a given name
        /// </summary>
        /// <param name="callPoint"></param>
        /// <param name="callType">defaults to Point</param>
        void LogSpecificName(string callPoint, CallTypes callType = CallTypes.Point);

        /// <summary>
        /// Thsi will log the name of the calling method
        /// </summary>
        /// <param name="callType">defaults to Point</param>
        /// <param name="callerName">Do not use. Filled in by system with the calling method name</param>
        void LogCaller( CallTypes callType = CallTypes.Point, [CallerMemberName] string callerName = "");

        /// <summary>
        /// Optional method that will setup any mapping etc. that are cached. This will will improve speed later.
        /// The GenericDto will still work without this method being called, but the first use that needs the map will be slower. 
        /// </summary>
        void CacheSetup();
    }

    /// <summary>
    /// This is a copy of CommsTestActionDto just so we can try the TActionData to TDto versions
    /// </summary>
    public class GTestActionDto : InstrumentedEfGenericDto<GTestActionData, GTestActionDto>, IGTestActionDto
    {
        private double _secondsBetweenIterations = 1;
        private int _numIterations = 5;

        [UIHint("Enum")]
        public TestServiceModes Mode { get; set; }

        public bool FailToRespondToCancel { get; set; }

        public int NumErrorsToExitWith { get; set; }

        [Range(0, 100)]
        public double SecondsBetweenIterations
        {
            get { return _secondsBetweenIterations; }
            set { _secondsBetweenIterations = value; }
        }

        public int NumIterations
        {
            get { return _numIterations; }
            set { _numIterations = value; }
        }

        public double SecondsDelayToRespondingToCancel { get; set; }

        protected internal override ServiceFunctions SupportedFunctions
        {
            get { return ServiceFunctions.DoAction | ServiceFunctions.DoDbAction; }
        }
    }
}