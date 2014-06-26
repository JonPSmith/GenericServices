using System.Threading.Tasks;
using GenericServices.Core;

namespace GenericServices.ServicesAsync
{
    public interface IActionServiceAsync<TActionOut, in TActionIn>
    {
        /// <summary>
        /// This runs a action that returns a result. 
        /// </summary>
        /// <param name="actionComms">The actionComms to allow progress reports and cancellation</param>
        /// <param name="actionData">Data that the action takes in to undertake the action</param>
        /// <returns>The status, with a result if Valid</returns>
        Task<ISuccessOrErrors<TActionOut>> DoActionAsync(IActionComms actionComms, TActionIn actionData);
    }
}

namespace GenericServices.ServicesAsync
{
    public interface IActionServiceAsync<TActionOut, TActionIn, TDto> 
        where TActionIn : class, new()
        where TDto : EfGenericDtoAsync<TActionIn, TDto>
    {
        /// <summary>
        /// This runs an action that does not write to the database. 
        /// It first converts the dto to the TActionIn format and then runs the action
        /// </summary>
        /// <param name="actionComms">The actioncomms to allow progress reports and cancellation</param>
        /// <param name="dto">The dto to be converted to the TActionIn class</param>
        /// <returns>The status, with a result if the status is valid</returns>
        Task<ISuccessOrErrors<TActionOut>> DoActionAsync(IActionComms actionComms, TDto dto);

        /// <summary>
        /// This is available to reset any secondary data in the dto. Call this if the ModelState was invalid and
        /// you need to display the view again with errors
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<TDto> ResetDtoAsync(TDto dto);
    }
}