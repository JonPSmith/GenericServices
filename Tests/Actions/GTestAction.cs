using System;
using System.Diagnostics;
using System.Threading;
using GenericServices;
using GenericServices.Actions;
using GenericServices.Core;
using GenericServices.Services;

namespace Tests.Actions
{
    public interface IGTestAction : IActionSync<int, GTestActionData>, IDisposable
    {

    }

    public class GTestAction : ActionBase, IGTestAction
    {
        /// <summary>
        /// This allows the action to configure what it supports, which then affects what the user sees
        /// Note: it must be a constant as it is read just after the action is created
        /// </summary>
        public override ActionFlags ActionConfig
        {
            get { return ActionFlags.Normal; }
        }

        /// <summary>
        /// If true then the caller should call EF SubmitChanges if the method exited with status IsValid and
        /// it looks to see if the data part has a ICheckIfWarnings and if the WriteEvenIfWarning is false
        /// and there are warnings then it does not call SubmitChanges
        /// </summary>
        public override bool SubmitChangesOnSuccess { get { return false; } }

        public bool DisposeWasCalled { get; private set; }

        public ISuccessOrErrors<int> DoAction(IActionComms actionComms, GTestActionData dto)
        {
            ISuccessOrErrors<int> result = new SuccessOrErrors<int>();

            if (dto.Mode == TestServiceModes.ThrowExceptionOnStart)
                throw new Exception("Thrown exception at start.");

            DateTime startTime = DateTime.Now;

            ReportProgressAndThrowExceptionIfCancelPending(actionComms, 0, 
                new ProgressMessage(ProgressMessageTypes.Info, "Action has started. Will run for {0:f1} seconds.", dto.NumIterations * dto.SecondsBetweenIterations));

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
                //Thread.Sleep( (int)(dto.SecondsBetweenIterations * 1000));
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
                result.SetSuccessWithResult(dto.NumIterations,
                    string.Format("Have completed the action in {0:F2} seconds",
                    DateTime.Now.Subtract(startTime).TotalSeconds));
            }

            return result;
        }

        /// <summary>
        /// If the user wants something to be called at the end then adding IDisposable to the class ensures 
        /// that whatever happens Dispose on the action weill be called at the end
        /// </summary>
        public void Dispose()
        {
            DisposeWasCalled = true;
            Debug.WriteLine("The dispose was called");
        }

    }
}