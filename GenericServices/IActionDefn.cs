namespace GenericServices
{
    public interface IActionDefn<in T>
    {
        /// <summary>
        /// This controls the lower value sent back to reportProgress
        /// Lower and Upper bound are there to allow outer tasks to call inner tasks 
        /// to do part of the work and still report progress properly
        /// </summary>
        int LowerBound { get; set; }

        /// <summary>
        /// This controls the upper bound of the value sent back to reportProgress
        /// </summary>
        int UpperBound { get; set; }

        /// <summary>
        /// This is a general form of a task that can be run sync or async
        /// </summary>
        /// <param name="actionComms">Action communication channel, can be null</param>
        /// <param name="actionData">setup data sent to the service </param>
        /// <returns></returns>
        ISuccessOrErrors DoAction(IActionComms actionComms, T actionData);

    }
}
