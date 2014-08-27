using System.Threading.Tasks;

namespace GenericServices
{
    public interface IUpdateSetupServiceAsync
    {
        /// <summary>
        /// This returns a status which, if Valid, has single entry using the primary keys to find it.
        /// </summary>
        /// <typeparam name="T">The type of the data to output. 
        /// Type must be a type either an EF data class or one of the EfGenericDto's</typeparam>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Task with Status. If valid Result holds data (not tracked), otherwise null</returns>
        Task<ISuccessOrErrors<T>> GetOriginalAsync<T>(params object[] keys) where T : class;
    }
}