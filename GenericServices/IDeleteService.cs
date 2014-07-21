namespace GenericServices
{
    public interface IDeleteService
    {
        /// <summary>
        /// This will delete an item from the database
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns></returns>
        ISuccessOrErrors Delete<TData>(params object[] keys) where TData : class, new();
    }
}