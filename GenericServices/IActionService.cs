using GenericServices.Services;

namespace GenericServices
{
    public interface IActionService<TAction, in TActionData>
        where TAction : class, IActionDefn<TActionData> 
    {
        /// <summary>
        /// This runs a task that does not write to the database. 
        /// We assume it passes information back via the taskData
        /// </summary>
        /// <param name="taskData"></param>
        /// <returns></returns>
        ISuccessOrErrors DoAction(TActionData taskData);

        /// <summary>
        /// This runs a task that writes data to the database. 
        /// It calls SaveChangesWithValidation to commit the data as long as there are
        /// no errors and (no warnings or warnings don't matter)
        /// </summary>
        /// <param name="taskData"></param>
        /// <returns></returns>
        ISuccessOrErrors DoDbAction(TActionData taskData);

        /// <summary>
        /// This runs a task that does not write to the database. We assume is passes data back via the dto.
        /// It first converts the dto to the taskdata format, runs the task and then converts
        /// the taskdata back to the dto format
        /// </summary>
        /// <param name="taskComms"></param>
        /// <param name="taskData"></param>
        /// <returns></returns>
        ISuccessOrErrors DoAction(IActionComms taskComms, TActionData taskData);

        /// <summary>
        /// This runs a task that does not write to the database. We assume is passes data back via the dto.
        /// It first converts the dto to the taskdata format, runs the task and then converts
        /// the taskdata back to the dto format
        /// </summary>
        /// <param name="taskComms"></param>
        /// <param name="taskData"></param>
        /// <returns></returns>
        ISuccessOrErrors DoDbAction(IActionComms taskComms, TActionData taskData);
    }

    public interface IActionService<TAction, TActionData, TDto>
        where TAction : class, IActionDefn<TActionData>
        where TActionData : class, new()
        where TDto : EfGenericDto<TActionData, TDto>
    {
        /// <summary>
        /// This runs a task that does not write to the database. We assume is passes data back via the dto.
        /// It first converts the dto to the taskdata format, runs the task and then converts
        /// the taskdata back to the dto format
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        ISuccessOrErrors DoAction(TDto dto);

        /// <summary>
        /// This converts the dto to the task data format then runs the task
        /// The task is assumed to have produced updated data to go into the database. 
        /// It calls SaveChangesWithValidation to commit the data as long as there are
        /// no errors and (no warnings || warnings don't matter)
        /// </summary>
        /// <param name="dto">The dto to be converted to the ActionData format</param>
        /// <returns></returns>
        ISuccessOrErrors DoDbAction(TDto dto);

        /// <summary>
        /// This runs a task that does not write to the database. We assume is passes data back via the dto.
        /// It first converts the dto to the taskdata format, runs the task and then converts
        /// the taskdata back to the dto format
        /// </summary>
        /// <param name="taskComms"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        ISuccessOrErrors DoAction(IActionComms taskComms, TDto dto);

        /// <summary>
        /// This runs a task that does not write to the database. We assume is passes data back via the dto.
        /// It first converts the dto to the taskdata format, runs the task and then converts
        /// the taskdata back to the dto format
        /// </summary>
        /// <param name="taskComms"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        ISuccessOrErrors DoDbAction(IActionComms taskComms, TDto dto);

        /// <summary>
        /// This is available to reset any secondary data in the dto. Call this if the ModelState was invalid and
        /// you need to display the view again with errors
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        TDto ResetDto(TDto dto);
    }
}