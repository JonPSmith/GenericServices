using System;
using System.Diagnostics;
using System.Threading;
using GenericServices;
using GenericServices.Actions;
using GenericServices.Concrete;

namespace Tests.Actions
{
    public interface ICommsTestAction : IActionDefn<CommsTestActionDto>, IDisposable
    {

    }

    public class CommsTestAction : ActionBase, ICommsTestAction
    {

        public bool DisposeWasCalled { get; private set; }

        public ISuccessOrErrors DoAction(IActionComms actionComms, CommsTestActionDto dto)
        {
            var result = new SuccessOrErrors();

            if (dto.Mode == TestServiceModes.ThrowExceptionOnStart)
                throw new Exception("Thrown exception at start.");

            DateTime startTime = DateTime.Now;

            ReportProgressAndThrowExceptionIfCancelPending(actionComms, 0, 
                new ProgressMessage(ProgressMessageTypes.Info, "Task has started. Will run for {0:f1} seconds.", dto.NumIterations * dto.SecondsBetweenIterations));

            for (int i = 0; i < dto.NumIterations; i++)
            {
                if (dto.Mode == TestServiceModes.ThrowExceptionHalfWayThrough &&  (i+1)/2 >= dto.NumIterations/2)
                    throw new Exception("Thrown exception half way through.");
                if (dto.Mode == TestServiceModes.ThrowOperationCanceledExceptionHalfWayThrough && (i + 1) / 2 >= dto.NumIterations / 2)
                    throw new OperationCanceledException();         //we simulate a cancel half way through work

                ReportProgress(actionComms, (i + 1)*100/dto.NumIterations,
                    new ProgressMessage(dto.Mode == TestServiceModes.RunButOutputErrors && (i%2 == 0)
                        ? ProgressMessageTypes.Error
                        : ProgressMessageTypes.Info,
                        string.Format("Iteration {0} of {1} done.", i + 1, dto.NumIterations)));
                if (CancelPending(actionComms))
                {
                    if (!dto.FailToRespondToCancel)
                    {
                        //we will respond to cancel
                        if (dto.SecondsDelayToRespondingToCancel > 0)
                            //... but with an additional delay
                            Thread.Sleep((int)(dto.SecondsDelayToRespondingToCancel * 1000));

                        return result.AddSingleError("Cancelled by user.");
                    }
                }

                Thread.Sleep( (int)(dto.SecondsBetweenIterations * 1000));
            }

            if (dto.Mode == TestServiceModes.RunButOutputOneWarningAtEnd)
                result.AddWarning("The mode was set to RunButOutputOneWarningAtEnd.");

            if (dto.NumErrorsToExitWith > 0)
            {
                for (int i = 0; i < dto.NumErrorsToExitWith; i++)
                        result.AddSingleError(string.Format(
                            "Error {0}: You asked me to declare an error when finished.", i));
            }
            else
            {
                result.SetSuccessMessage(string.Format("Have completed the task in {0:F2} seconds",
                                                           DateTime.Now.Subtract(startTime).TotalSeconds)); 
            }


            return result;
        }

        /// <summary>
        /// If the user wants something to be called at the end then adding IDisposable to the class ensures 
        /// that whatever happens Dispose on the task weill be called at the end
        /// </summary>
        public void Dispose()
        {
            DisposeWasCalled = true;
            Debug.WriteLine("The dispose was called");
        }
    }
}