using GenericServices.ActionComms;

namespace GenericServices
{

    public interface IActionSync<TOut, in TIn> : IActionBase
    {
        /// <summary>
        /// This is a general form of a method to be run
        /// </summary>
        /// <param name="actionData">setup data sent to the service </param>
        /// <returns></returns>
        ISuccessOrErrors<TOut> DoAction(TIn actionData);
    }
}
