using System;
using GenericServices.Actions;
using GenericServices.Services;

namespace GenericServices.Services
{
    public class ActionService<TAction, TActionData> : IActionService<TAction, TActionData> 
        where TAction : class, IActionDefn<TActionData> 
    {
        private readonly TAction _taskToRun;

        public ActionService(TAction taskToRun)
        {
            if (taskToRun == null)
                throw new NullReferenceException("Dependecy injection did not find the action. Check you have added IActionService<ActionDto> to the classe's interface.");
            _taskToRun = taskToRun;
        }

        /// <summary>
        /// This runs a task that does not write to the database. 
        /// We assume it passes information back via the taskData
        /// </summary>
        /// <param name="taskData"></param>
        /// <returns></returns>
        public ISuccessOrErrors DoAction(TActionData taskData)
        {
            return DoAction(null, taskData);
        }

        /// <summary>
        /// This runs a task that does not write to the database. 
        /// We assume it passes information back via the taskData
        /// </summary>
        /// <param name="taskComms">The taskcomms to allow progress reports and cancellation</param>
        /// <param name="taskData"></param>
        /// <returns></returns>
        public ISuccessOrErrors DoAction(IActionComms taskComms, TActionData taskData)
        {

            try
            {
                return _taskToRun.DoAction(taskComms, taskData);
            }
            finally
            {
                var disposable = _taskToRun as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }
    }

    //---------------------------------------------------------------------------


    public class ActionService<TAction, TActionData, TDto> : IActionService<TAction, TActionData, TDto>
        where TAction : class, IActionDefn<TActionData>
        where TActionData : class, new()
        where TDto : EfGenericDto<TActionData, TDto>
    {

        private readonly IDbContextWithValidation _db;
        private readonly TAction _taskToRun;

        public ActionService(IDbContextWithValidation db, TAction taskToRun)
        {
            if (taskToRun == null)
                throw new NullReferenceException("Dependecy injection did not find the action. Check you have added IActionService<ActionDto> to the classe's interface.");
            _db = db;
            _taskToRun = taskToRun;
        }

        /// <summary>
        /// This runs a task that does not write to the database. We assume is passes data back via the dto.
        /// It first converts the dto to the taskdata format, runs the task and then converts
        /// the taskdata back to the dto format
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ISuccessOrErrors DoAction(TDto dto)
        {
            var actionStatus = DoAction(null, dto);
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                //we reset any secondary data as we expect the view to be reshown with the errors
                dto.SetupSecondaryData(_db, dto);

            return actionStatus;
        }

        //--------------------------------------------------------------------------------
        //now the versions with comms

        /// <summary>
        /// This runs a task that does not write to the database. We assume is passes data back via the dto.
        /// It first converts the dto to the taskdata format, runs the task and then converts
        /// the taskdata back to the dto format
        /// </summary>
        /// <param name="taskComms">The taskcomms to allow progress reports and cancellation</param>
        /// <returns></returns>
        public ISuccessOrErrors DoAction(IActionComms taskComms, TDto dto)
        {
            ISuccessOrErrors status = new SuccessOrErrors();

            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoAction))
                return status.AddSingleError("Running a task is not setup for this data.");

            var taskData = new TActionData();
            status = dto.CopyDtoToData(_db, dto, taskData); //convert Tdto into TActionData format
            if (!status.IsValid) return status;

            try
            {
                status = _taskToRun.DoAction(taskComms, taskData);
            }
            finally
            {
                var disposable = _taskToRun as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
            if (!status.IsValid) return status;

            var copyStatus = dto.CopyDataToDto(_db, taskData, dto); //now convert back into Dto format
            if (!copyStatus.IsValid) status = copyStatus;      //we send back the bad copystatus

            if (taskComms != null) taskComms.ReportProgress(100);

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