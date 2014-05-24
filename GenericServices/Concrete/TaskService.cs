using System;
using GenericServices.Tasking;

namespace GenericServices.Concrete
{


    public class TaskService<TTask, TTaskData> : ITaskService<TTask, TTaskData> 
         where TTask : class, ITaskService<TTaskData> 
    {
        private readonly IDbContextWithValidation _db;
        private readonly TTask _taskToRun;

        public TaskService(IDbContextWithValidation db, TTask taskToRun)
        {
            if (taskToRun == null)
                throw new NullReferenceException("Dependecy injection did not find the task. Check you have added ITaskService<TaskDto> to the task's interface.");
            _db = db;
            _taskToRun = taskToRun;
        }

        /// <summary>
        /// This runs a task that does not write to the database. 
        /// We assume it passes information back via the taskData
        /// </summary>
        /// <param name="taskData"></param>
        /// <returns></returns>
        public ISuccessOrErrors RunTask(TTaskData taskData)
        {
            return _taskToRun.Task(null, taskData);
        }

        /// <summary>
        /// This runs a task that writes data to the database. 
        /// It calls SaveChangesWithValidation to commit the data as long as there are
        /// no errors and (no warnings or warnings don't matter)
        /// </summary>
        /// <param name="taskData"></param>
        /// <returns></returns>
        public ISuccessOrErrors RunDbTask(TTaskData taskData)
        {

            var taskStatus = _taskToRun.Task(null, taskData);
            if (!taskStatus.IsValid) return taskStatus;             //task failed

            if (TaskServiceHelper.ShouldStopAsWarningsMatter(taskStatus.HasWarnings, taskData))
                //There were warnings and we are asked to not write to the database
                return taskStatus.SetSuccessMessage("{0}... but NOT written to database as warnings.",
                    taskStatus.SuccessMessage);

            //we now need to save the changes to the database
            var dataStatus = _db.SaveChangesWithValidation();
            return dataStatus.IsValid 
                ? dataStatus.SetSuccessMessage("{0}... and written to database.", taskStatus.SuccessMessage)
                : dataStatus;
        }

    }

    //---------------------------------------------------------------------------



    public class TaskService<TTask, TTaskData, TDto> : ITaskService<TTask, TTaskData, TDto> 
        where TTask : class, ITaskService<TTaskData> 
        where TTaskData : class, new()
        where TDto : EfGenericDto<TTaskData, TDto>
    {

        private readonly IDbContextWithValidation _db;
        private readonly TTask _taskToRun;

        public TaskService(IDbContextWithValidation db, TTask taskToRun)
        {
            if (taskToRun == null)
                throw new NullReferenceException("Dependecy injection did not find the task. Check you have added ITaskService<TaskDto> to the task's interface.");
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
        public ISuccessOrErrors RunTask(TDto dto)
        {
            var status = RunTask(dto, null);
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                //we reset any secondary data as we expect the view to be reshown with the errors
                dto.SetupSecondaryData(_db, dto);

            return status;
        }


        /// <summary>
        /// This converts the dto to the task data format then runs the task
        /// The task is assumed to have produced updated data to go into the database. 
        /// It calls SaveChangesWithValidation to commit the data as long as there are
        /// no errors and (no warnings or warnings don't matter)
        /// </summary>
        /// <param name="dto">The dto to be converted to the TaskData format</param>
        /// <returns></returns>
        public ISuccessOrErrors RunDbTask(TDto dto)
        {
            var status = RunDbTask(dto, null);
            if (!status.IsValid && !dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                //we reset any secondary data as we expect the view to be reshown with the errors
                dto.SetupSecondaryData(_db, dto);

            return status;
        }


        //--------------------------------------------------------------------------------
        //now the protected versions

        /// <summary>
        /// This runs a task that does not write to the database. We assume is passes data back via the dto.
        /// It first converts the dto to the taskdata format, runs the task and then converts
        /// the taskdata back to the dto format
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="taskComms"></param>
        /// <returns></returns>
        private ISuccessOrErrors RunTask(TDto dto, ITaskComms taskComms)
        {
            ISuccessOrErrors status = new SuccessOrErrors();

            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.RunTask))
                return status.AddSingleError("Running a task is not setup for this data.");

            var taskData = new TTaskData();
            status = dto.CopyDtoToData(_db, dto, taskData); //convert Tdto into TTaskData format
            if (!status.IsValid) return status;

            status = _taskToRun.Task(taskComms, taskData);
            if (!status.IsValid) return status;

            var copyStatus = dto.CopyDataToDto(_db, taskData, dto); //now convert back into Dto format
            if (!copyStatus.IsValid) status = copyStatus;      //we send back the bad copystatus

            if (taskComms != null) taskComms.ReportProgress(100);

            return status;      
        }


        private ISuccessOrErrors RunDbTask(TDto dto, ITaskComms taskComms)
        {
            ISuccessOrErrors status = new SuccessOrErrors();

            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.RunDbTask))
                return status.AddSingleError("Running a db task is not setup for this data.");

            _taskToRun.UpperBound = 90; //we need to write to database, so lower task % to allow for final commit

            var taskData = new TTaskData();
            status = dto.CopyDtoToData(_db, dto, taskData); //convert Tdto into TTaskData format
            if (!status.IsValid) return status;

            status = _taskToRun.Task(taskComms, taskData);
            if (!status.IsValid) return status;

            if (TaskServiceHelper.ShouldStopAsWarningsMatter(status.HasWarnings, dto))
                //There were warnings and we are asked to not write to the database
                return status.SetSuccessMessage("{0}... but NOT written to database as warnings.",
                    status.SuccessMessage);

            //if get to here we need to write to database, so handle that with possible task comms
            var taskSuccessMessage = status.SuccessMessage;
            if (taskComms != null)
                taskComms.ReportProgress(90, TaskMessage.InfoMessage("Writing to database..."));
            status = _db.SaveChangesWithValidation();
            if (!status.IsValid) return status;

            if (taskComms != null)
                taskComms.ReportProgress(100, TaskMessage.InfoMessage("Successfully written to database."));
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

    internal static class TaskServiceHelper
    {

        internal static bool ShouldStopAsWarningsMatter<T>(bool hasWarnings, T classToCheck)
        {
            if (!hasWarnings) return false;
            var flagClass = classToCheck as ICheckIfWarnings;
            return (flagClass != null && !flagClass.WriteEvenIfWarning);
        }
    }
}
