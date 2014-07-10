using System.Threading.Tasks;
using GenericServices.ActionComms;

namespace GenericServices
{

    public interface IActionAsync<TOut, in TIn> : IActionBase
    {
        /// <summary>
        /// This is a general form of a method to be run
        /// </summary>
        /// <param name="actionData">setup data sent to the service </param>
        /// <returns></returns>
        Task<ISuccessOrErrors<TOut>> DoActionAsync(TIn actionData);
    }
}
