using GenericServices.Core;

namespace GenericServices
{
    public interface IActionService<TActionOut, in TActionIn>
    {
        /// <summary>
        /// This runs a method, handing it the data it needs
        /// </summary>
        /// <param name="taskData"></param>
        /// <returns>The status, with a result if the status is valid</returns>
        ISuccessOrErrors<TActionOut> DoAction(TActionIn taskData);
    }

    public interface IActionService<TActionOut, TActionIn, TDto>
        where TActionIn : class, new()
        where TDto : EfGenericDto<TActionIn, TDto>
    {
        /// <summary>
        /// This converts the dto to the format that the method needs and then runs it
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>The status, with a result if the status is valid</returns>
        ISuccessOrErrors<TActionOut> DoAction(TDto dto);

        /// <summary>
        /// This is available to reset any secondary data in the dto. Call this if the ModelState was invalid and
        /// you need to display the view again with errors
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        TDto ResetDto(TDto dto);
    }
}