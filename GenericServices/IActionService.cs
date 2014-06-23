using GenericServices.Services;

namespace GenericServices
{
    public interface IActionService<TAction, TActionOut, in TActionIn>
        where TAction : class, IActionDefn<TActionOut, TActionIn> 
    {

        /// <summary>
        /// This runs a task that does not write to the database. We assume is passes data back via the dto.
        /// It first converts the dto to the taskdata format, runs the task and then converts
        /// the taskdata back to the dto format
        /// </summary>
        /// <param name="taskComms"></param>
        /// <param name="taskData"></param>
        /// <returns></returns>
        ISuccessOrErrors<TActionOut> DoAction(IActionComms taskComms, TActionIn taskData);

    }

    public interface IActionService<TAction, TActionOut, TActionIn, TDto>
        where TAction : class, IActionDefn<TActionOut, TActionIn>
        where TActionIn : class, new()
        where TDto : EfGenericDto<TActionIn, TDto>
    {

        /// <summary>
        /// This runs a task that does not write to the database. We assume is passes data back via the dto.
        /// It first converts the dto to the taskdata format, runs the task and then converts
        /// the taskdata back to the dto format
        /// </summary>
        /// <param name="taskComms"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        ISuccessOrErrors<TActionOut> DoAction(IActionComms taskComms, TDto dto);

        /// <summary>
        /// This is available to reset any secondary data in the dto. Call this if the ModelState was invalid and
        /// you need to display the view again with errors
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        TDto ResetDto(TDto dto);
    }
}