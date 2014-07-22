using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericServices.Core;

namespace GenericServices.ServicesAsync
{

    public interface IUpdateSetupServiceAsync<TData> where TData : class
    {
        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Data class as read from database (not tracked)</returns>
        Task<TData> GetOriginalUsingWhereAsync(Expression<Func<TData, bool>> whereExpression);

        /// <summary>
        /// This finds an entry using the primary key(s) in the data
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Data class as read from database (not tracked)</returns>
        Task<TData> GetOriginalAsync(params object[] keys);
    }
}