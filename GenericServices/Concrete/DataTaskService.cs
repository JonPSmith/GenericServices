using System;
using GenericServices.Tasking;

namespace GenericServices.Concrete
{
    public class DataTaskService<TTask, TTaskData> 
        where TTask : class, ITaskService<TTaskData> 
        where TTaskData : class
    {
        private readonly IDbContextWithValidation _db;
        private readonly TTask _taskToRun;

        public DataTaskService(IDbContextWithValidation db, TTask taskToRun)
        {
            if (taskToRun == null)
                throw new NullReferenceException("Dependecy injection did not find the task. Check you have added ITaskService<TaskDto> to the task's definition.");
            _db = db;
            _taskToRun = taskToRun;
        }

        public ISuccessOrErrors RunTask(TTaskData taskData)
        {

            var taskStatus = _taskToRun.Task(null, taskData);
            if (!taskStatus.IsValid) return taskStatus;             //task failed

            if (taskStatus.HasWarnings)
            {
                //see if we care about this
                var flagClass = taskData as ICheckIfWarnings;
                if (flagClass != null && !flagClass.WriteEvenIfWarning)
                    return taskStatus.SetSuccessMessage("{0}... but NOT written to database as warnings.",
                        taskStatus.SuccessMessage);
            }
                
            //we now need to save the changes to the database
            var dataStatus = _db.SaveChangesWithValidation();
            return dataStatus.IsValid 
                ? dataStatus.SetSuccessMessage("{0} and written to database", taskStatus.SuccessMessage)
                : dataStatus;
        }

    }

    //---------------------------------------------------------------------------

    public class DataTaskService<TTask, TTaskData, TDto>
        where TTask : class, ITaskService<TTaskData> 
        where TTaskData : class, new()
        where TDto : EfGenericDto<TTaskData, TDto>
    {

        private readonly IDbContextWithValidation _db;
        private readonly TTask _taskToRun;

        public DataTaskService(IDbContextWithValidation db, TTask taskToRun)
        {
            if (taskToRun == null)
                throw new NullReferenceException("Dependecy injection did not find the task. Check you have added ITaskService<TaskDto> to the task's definition.");
            _db = db;
            _taskToRun = taskToRun;
        }

        public ISuccessOrErrors RunTask(TDto dto)
        {
            return RunTask(dto, null);
        }

        protected ISuccessOrErrors RunTask(TDto dto, ITaskComms taskComms)
        {
            ISuccessOrErrors status = new SuccessOrErrors();

            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.RunDataTask))
                return status.AddSingleError("Running a task is not setup for this data.");

            _taskToRun.UpperBound = 90; //we need to write to database, so lower task % to allow for final commit

            var taskData = new TTaskData();
            status = dto.CopyDtoToData(_db, dto, taskData); //convert Tdto into TTaskData format
            if (!status.IsValid) goto exitWithErrors;

            status = _taskToRun.Task(taskComms, taskData);
            if (!status.IsValid) goto exitWithErrors;

            if (status.HasWarnings)
            {
                //see if we care about this
                var flagClass = taskData as ICheckIfWarnings;
                if (flagClass != null && !flagClass.WriteEvenIfWarning)
                    return status.SetSuccessMessage("{0}.. but NOT written to database as warnings.",
                        status.SuccessMessage);
            }

            //if get to here we need to write to database, so handle that with possible task comms
            var taskSuccessMessage = status.SuccessMessage;
            if (taskComms != null)
                taskComms.ReportProgress(90, TaskMessage.InfoMessage("Writing to database..."));
            status = _db.SaveChangesWithValidation();
            if (!status.IsValid) goto exitWithErrors;

            if (taskComms != null)
                taskComms.ReportProgress(100, TaskMessage.InfoMessage("Successfully written to database."));
            return status.SetSuccessMessage("{0}.. and written to database.", taskSuccessMessage);

            //***** otherwise there are errors *****
exitWithErrors:
            if (taskComms == null &&!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                //we are running sync and there were errors so reset any secondary data as we expect the view to be reshown
                dto.SetupSecondaryData(_db, dto);
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
}
