using System;
using System.Threading.Tasks;
using GenericServices.Actions.Internal;
using GenericServices.Core;
using GenericServices.Services;

namespace GenericServices.ServicesAsync
{
    public class ActionServiceAsync<TActionOut, TActionIn> : IActionServiceAsync<TActionOut, TActionIn> 
    {
        private readonly IDbContextWithValidation _db;
        private readonly IActionAsync<TActionOut, TActionIn> _actionToRun;

        public ActionServiceAsync(IDbContextWithValidation db, IActionAsync<TActionOut, TActionIn> actionToRun)
        {
            if (actionToRun == null)
                throw new NullReferenceException("Dependecy injection did not find the action. Check you have added IActionSync<TActionOut, TActionIn> to the classe's interface.");
            _db = db;
            _actionToRun = actionToRun;
        }

        /// <summary>
        /// This runs a action that returns a result. 
        /// </summary>
        /// <param name="actionComms">The actionComms to allow progress reports and cancellation</param>
        /// <param name="actionData">Data that the action takes in to undertake the action</param>
        /// <returns>A Task containing status, which has a result if Valid</returns>
        public async Task<ISuccessOrErrors<TActionOut>> DoActionAsync(IActionComms actionComms, TActionIn actionData)
        {

            try
            {
                var status = await _actionToRun.DoActionAsync(actionComms, actionData);
                return status.AskedToSaveChanges(_actionToRun)
                    ? await status.SaveChangesAttemptAsync(actionData, _db)
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

    public class ActionServiceAsync<TActionOut, TActionIn, TDto> : IActionServiceAsync<TActionOut, TActionIn, TDto>
        where TActionIn : class, new()
        where TDto : EfGenericDtoAsync<TActionIn, TDto>
    {

        private readonly IDbContextWithValidation _db;
        private readonly IActionAsync<TActionOut, TActionIn> _actionToRun;

        public ActionServiceAsync(IDbContextWithValidation db, IActionAsync<TActionOut, TActionIn> actionToRun)
        {
            if (actionToRun == null)
                throw new NullReferenceException("Dependecy injection did not find the action. Check you have added IActionSync<TActionOut, TActionIn> to the classe's interface.");
            _db = db;
            _actionToRun = actionToRun;
        }

        /// <summary>
        /// This runs an action that does not write to the database. 
        /// It first converts the dto to the TActionIn format and then runs the action
        /// </summary>
        /// <param name="actionComms">The actioncomms to allow progress reports and cancellation</param>
        /// <param name="dto">The dto to be converted to the TActionIn class</param>
        /// <returns>A Task containing status, which has a result if Valid</returns>
        public async Task<ISuccessOrErrors<TActionOut>> DoActionAsync(IActionComms actionComms, TDto dto)
        {
            ISuccessOrErrors<TActionOut> status = new SuccessOrErrors<TActionOut>();

            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoActionWithoutValidate))
                return status.AddSingleError("Running an action is not setup for this data.");

            var actionInData = new TActionIn();
            var nonResultStatus = await dto.CopyDtoToDataAsync(_db, dto, actionInData); //convert Tdto into TActionIn format
            if (!nonResultStatus.IsValid) 
                return SuccessOrErrors<TActionOut>.ConvertNonResultStatus( nonResultStatus);

            try
            {
                status = await _actionToRun.DoActionAsync(actionComms, actionInData);
                return status.AskedToSaveChanges(_actionToRun)
                    ? await status.SaveChangesAttemptAsync(actionInData, _db)
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
        public async Task<TDto> ResetDtoAsync(TDto dto)
        {
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                //we reset any secondary data as we expect the view to be reshown with the errors
                await dto.SetupSecondaryDataAsync(_db, dto);

            return dto;
        }
    }

}