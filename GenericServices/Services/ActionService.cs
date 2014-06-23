using System;
using GenericServices.Actions;
using GenericServices.Services;

namespace GenericServices.Services
{
    public class ActionService<TAction, TActionOut, TActionIn>
        where TAction : class, IActionDefn<TActionOut, TActionIn>
    {
        private readonly TAction _actionToRun;

        public ActionService(TAction actionToRun)
        {
            if (actionToRun == null)
                throw new NullReferenceException("Dependecy injection did not find the action. Check you have added IActionService<TActionOut, TActionIn> to the classe's interface.");
            _actionToRun = actionToRun;
        }

        /// <summary>
        /// This runs a action that returns a result. 
        /// </summary>
        /// <param name="actionComms">The actionComms to allow progress reports and cancellation</param>
        /// <param name="actionData">Data that the action takes in to undertake the action</param>
        /// <returns>The status, with a result if Valid</returns>
        public ISuccessOrErrors<TActionOut> DoAction(IActionComms actionComms, TActionIn actionData)
        {

            try
            {
                return _actionToRun.DoAction(actionComms, actionData);
            }
            finally
            {
                var disposable = _actionToRun as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }
    }

    //---------------------------------------------------------------------------


    public class ActionService<TAction, TActionOut, TActionIn, TDto>
        where TAction : class, IActionDefn<TActionOut, TActionIn>
        where TActionIn : class, new()
        where TDto : EfGenericDto<TActionIn, TDto>
    {

        private readonly IDbContextWithValidation _db;
        private readonly TAction _actionToRun;

        public ActionService(IDbContextWithValidation db, TAction actionToRun)
        {
            if (actionToRun == null)
                throw new NullReferenceException("Dependecy injection did not find the action. Check you have added IActionService<ActionDto> to the classe's interface.");
            _db = db;
            _actionToRun = actionToRun;
        }

        /// <summary>
        /// This runs an action that does not write to the database. 
        /// It first converts the dto to the TActionIn format and then runs the action
        /// </summary>
        /// <param name="actionComms">The actioncomms to allow progress reports and cancellation</param>
        /// <param name="dto">The dto to be converted to the TActionIn class</param>
        /// <returns>The status, with a result if the status is valid</returns>
        public ISuccessOrErrors<TActionOut> DoAction(IActionComms actionComms, TDto dto)
        {
            ISuccessOrErrors<TActionOut> status = new SuccessOrErrors<TActionOut>();

            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoAction))
                return status.AddSingleError("Running an action is not setup for this data.");

            var actionInData = new TActionIn();
            var nonResultStatus = dto.CopyDtoToData(_db, dto, actionInData); //convert Tdto into TActionIn format
            if (!nonResultStatus.IsValid) 
                return SuccessOrErrors<TActionOut>.ConvertNonResultStatus( nonResultStatus);

            try
            {
                status = _actionToRun.DoAction(actionComms, actionInData);
            }
            finally
            {
                var disposable = _actionToRun as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }

            if (status.IsValid && actionComms != null) actionComms.ReportProgress(100);

            return status;
        }

        /// <summary>
        /// This is available to reset any secondary data in the dto. Call this if the ModelState was invalid and
        /// you need to display the view again with errors
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public TDto ResetDto(TDto dto)
        {
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                //we reset any secondary data as we expect the view to be reshown with the errors
                dto.SetupSecondaryData(_db, dto);

            return dto;
        }
    }

    internal static class ActionServiceHelper
    {

        internal static bool ShouldStopAsWarningsMatter<T>(bool hasWarnings, T classToCheck)
        {
            if (!hasWarnings) return false;
            var flagClass = classToCheck as ICheckIfWarnings;
            return (flagClass != null && !flagClass.WriteEvenIfWarning);
        }
    }
}