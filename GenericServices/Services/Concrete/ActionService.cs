using System;
using GenericServices.Actions.Internal;
using GenericServices.Core;

namespace GenericServices.Services.Concrete
{
    public class ActionService<TActionOut, TActionIn> : IActionService<TActionOut, TActionIn>
    {
        private readonly IDbContextWithValidation _db;
        private readonly IActionSync<TActionOut, TActionIn> _actionToRun;

        public ActionService(IDbContextWithValidation db, IActionSync<TActionOut, TActionIn> actionToRun)
        {
            if (actionToRun == null)
                throw new NullReferenceException(
                    "Dependecy injection did not find the action. Check you have added IActionSync<TActionOut, TActionIn> to the classe's interface.");
            _db = db;
            _actionToRun = actionToRun;
        }

        /// <summary>
        /// This runs a action that returns a result. 
        /// </summary>
        /// <param name="actionData">Data that the action takes in to undertake the action</param>
        /// <returns>The status, with a result if Valid</returns>
        public ISuccessOrErrors<TActionOut> DoAction(TActionIn actionData)
        {

            try
            {
                var status = _actionToRun.DoAction(actionData);
                return status.AskedToSaveChanges(_actionToRun)
                    ? status.SaveChangesAttempt(actionData, _db)
                    : status;
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
    //DTO version

    public class ActionService<TActionOut, TActionIn, TDto> : IActionService<TActionOut, TActionIn, TDto>
        where TActionIn : class, new()
        where TDto : EfGenericDto<TActionIn, TDto>, new()
    {

        private readonly IDbContextWithValidation _db;
        private readonly IActionSync<TActionOut, TActionIn> _actionToRun;

        public ActionService(IDbContextWithValidation db, IActionSync<TActionOut, TActionIn> actionToRun)
        {
            if (actionToRun == null)
                throw new NullReferenceException(
                    "Dependecy injection did not find the action. Check you have added IActionSync<TActionOut, TActionIn> to the classe's interface.");
            _db = db;
            _actionToRun = actionToRun;
        }

        /// <summary>
        /// This runs an action that does not write to the database. 
        /// It first converts the dto to the TActionIn format and then runs the action
        /// </summary>
        /// <param name="dto">The dto to be converted to the TActionIn class</param>
        /// <returns>The status, with a result if the status is valid</returns>
        public ISuccessOrErrors<TActionOut> DoAction(TDto dto)
        {
            ISuccessOrErrors<TActionOut> status = new SuccessOrErrors<TActionOut>();

            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoActionWithoutValidate))
                return status.AddSingleError("Running an action is not setup for this data.");

            var actionInData = new TActionIn();
            var nonResultStatus = dto.CopyDtoToData(_db, dto, actionInData); //convert Tdto into TActionIn format
            if (!nonResultStatus.IsValid)
                return SuccessOrErrors<TActionOut>.ConvertNonResultStatus(nonResultStatus);

            try
            {
                status = _actionToRun.DoAction(actionInData);
                return status.AskedToSaveChanges(_actionToRun)
                    ? status.SaveChangesAttempt(actionInData, _db)
                    : status;
            }
            finally
            {
                var disposable = _actionToRun as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
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

}