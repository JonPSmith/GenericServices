using System;
using GenericServices.Actions;
using GenericServices.Services;

namespace GenericServices.Services
{
    public class ActionDbService<TAction, TActionData> : IActionDbService<TAction, TActionData> 
        where TAction : class, IActionDefn<TActionData> 
    {
        private readonly IDbContextWithValidation _db;
        private readonly TAction _taskToRun;

        public ActionDbService(IDbContextWithValidation db, TAction taskToRun)
        {
            if (taskToRun == null)
                throw new NullReferenceException("Dependecy injection did not find the action. Check you have added IActionService<ActionDto> to the classe's interface.");
            _db = db;
            _taskToRun = taskToRun;
        }


        /// <summary>
        /// This runs a task that writes data to the database. 
        /// It calls SaveChangesWithValidation to commit the data as long as there are
        /// no errors and (no warnings or warnings don't matter)
        /// </summary>
        /// <param name="taskData"></param>
        /// <returns></returns>
        public ISuccessOrErrors DoDbAction(TActionData taskData)
        {
            return DoDbAction(null, taskData);
        }

        /// <summary>
        /// This runs a task that writes data to the database. 
        /// It calls SaveChangesWithValidation to commit the data as long as there are
        /// no errors and (no warnings or warnings don't matter)
        /// </summary>
        /// <param name="taskComms">The taskcomms to allow progress reports and cancellation</param>
        /// <param name="taskData"></param>
        /// <returns></returns>
        public ISuccessOrErrors DoDbAction(IActionComms taskComms, TActionData taskData)
        {
            ISuccessOrErrors actionStatus;
            try
            {
                actionStatus = _taskToRun.DoAction(null, taskData);
            }
            finally
            {
                var disposable = _taskToRun as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
            if (!actionStatus.IsValid) return actionStatus;             //task failed

            if (ActionServiceHelper.ShouldStopAsWarningsMatter(actionStatus.HasWarnings, taskData))
                //There were warnings and we are asked to not write to the database
                return actionStatus.SetSuccessMessage("{0}... but NOT written to database as warnings.",
                    actionStatus.SuccessMessage);

            //we now need to save the changes to the database
            var dataStatus = _db.SaveChangesWithValidation();
            return dataStatus.IsValid 
                ? dataStatus.SetSuccessMessage("{0}... and written to database.", actionStatus.SuccessMessage)
                : dataStatus;
        }


    }

    //---------------------------------------------------------------------------



    public class ActionDbService<TAction, TActionData, TDto> : IActionDbService<TAction, TActionData, TDto>
        where TAction : class, IActionDefn<TActionData>
        where TActionData : class, new()
        where TDto : EfGenericDto<TActionData, TDto>
    {

        private readonly IDbContextWithValidation _db;
        private readonly TAction _taskToRun;

        public ActionDbService(IDbContextWithValidation db, TAction taskToRun)
        {
            if (taskToRun == null)
                throw new NullReferenceException("Dependecy injection did not find the action. Check you have added IActionService<ActionDto> to the classe's interface.");
            _db = db;
            _taskToRun = taskToRun;
        }

        /// <summary>
        /// This converts the dto to the task data format then runs the task
        /// The task is assumed to have produced updated data to go into the database. 
        /// It calls SaveChangesWithValidation to commit the data as long as there are
        /// no errors and (no warnings or warnings don't matter)
        /// </summary>
        /// <param name="dto">The dto to be converted to the ActionData format</param>
        /// <returns></returns>
        public ISuccessOrErrors DoDbAction(TDto dto)
        {
            var actionStatus = DoDbAction(null, dto);
            if (!actionStatus.IsValid && !dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                //we reset any secondary data as we expect the view to be reshown with the errors
                dto.SetupSecondaryData(_db, dto);

            return actionStatus;
        }

        //--------------------------------------------------------------------------------
        //now the versions with comms

        /// <summary>
        /// This runs a task that writes data to the database. 
        /// It calls SaveChangesWithValidation to commit the data as long as there are
        /// no errors and (no warnings or warnings don't matter)
        /// </summary>
        /// <param name="taskComms">The taskcomms to allow progress reports and cancellation</param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ISuccessOrErrors DoDbAction(IActionComms taskComms, TDto dto)
        {
            ISuccessOrErrors status = new SuccessOrErrors();

            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoDbAction))
                return status.AddSingleError("Running a db task is not setup for this data.");

            _taskToRun.UpperBound = 90; //we need to write to database, so lower task % to allow for final commit

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

            if (ActionServiceHelper.ShouldStopAsWarningsMatter(status.HasWarnings, dto))
                //There were warnings and we are asked to not write to the database
                return status.SetSuccessMessage("{0}... but NOT written to database as warnings.",
                    status.SuccessMessage);

            //if get to here we need to write to database, so handle that with possible task comms
            var taskSuccessMessage = status.SuccessMessage;
            if (taskComms != null)
                taskComms.ReportProgress(90, ProgressMessage.InfoMessage("Writing to database..."));
            status = _db.SaveChangesWithValidation();
            if (!status.IsValid) return status;

            if (taskComms != null)
                taskComms.ReportProgress(100, ProgressMessage.InfoMessage("Successfully written to database."));
            return status.SetSuccessMessage("{0}... and written to database.", taskSuccessMessage);

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