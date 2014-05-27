using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GenericServices;

namespace Tests.Actions
{

    public enum TestServiceModes
    {
        RunSuccessfully, RunButOutputErrors, RunButOutputOneWarningAtEnd,
        ThrowExceptionOnStart, ThrowExceptionHalfWayThrough,
        ThrowOperationCanceledExceptionHalfWayThrough
    }

    public class CommsTestActionData : ICheckIfWarnings
    {
        private double _secondsBetweenIterations = 1;
        private int _numIterations = 5;

        [UIHint("Enum")]
        public TestServiceModes Mode { get; set; }

        public bool FailToRespondToCancel { get; set; }

        public int NumErrorsToExitWith { get; set; }

        [Range(0,100)]
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

        public bool WriteEvenIfWarning { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            List<ValidationResult> result = new List<ValidationResult>();
            if (NumIterations <= 0 )
                result.Add(new ValidationResult("NumIterations cannot be zero or less", new[] { "NumIterations" }));

            return result;
        }


    }
}