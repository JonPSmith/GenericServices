using System.Threading.Tasks;

namespace GenericServices.ActionComms
{

    public interface IActionCommsAsync<TOut, in TIn> : IActionBase
    {
        /// <summary>
        /// This is a general form of a method to be run
        /// </summary>
        /// <param name="actionComms">Action communication channel, can be null</param>
        /// <param name="actionData">setup data sent to the service </param>
        /// <returns></returns>
        Task<ISuccessOrErrors<TOut>> DoActionAsync(IActionComms actionComms, TIn actionData);
    }
}
