using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericServices.Core;

namespace GenericServices.ServicesAsync
{

    public interface IUpdateSetupServiceAsync<TData> where TData : class, new()
    {
        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Task with Status. If valid Result holds data (not tracked), otherwise null</returns>
        Task<ISuccessOrErrors<TData>> GetOriginalUsingWhereAsync(Expression<Func<TData, bool>> whereExpression);

        /// <summary>
        /// This finds an entry using the primary key(s) in the data
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Task with Status. If valid Result holds data (not tracked), otherwise null</returns>
        Task<ISuccessOrErrors<TData>> GetOriginalAsync(params object[] keys);
    }

    public interface IUpdateSetupServiceAsync<TData, TDto>
        where TData : class, new()
        where TDto : EfGenericDtoAsync<TData, TDto>, new()
    {
        /// <summary>
        /// This gets a single entry using the lambda expression as a where part. It also calls
        /// the dto's SetupSecondaryData to setup any extra data needed
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Task with Status. If valid TDto type with properties copyed over and SetupSecondaryData called 
        /// to set secondary data, otherwise null</returns>
        Task<ISuccessOrErrors<TDto>> GetOriginalUsingWhereAsync(Expression<Func<TData, bool>> whereExpression);

        /// <summary>
        /// This returns a single entry using the primary keys to find it. It also calls
        /// the dto's SetupSecondaryData to setup any extra data needed
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Task with Status. If valid TDto type with properties copyed over and SetupSecondaryData called 
        /// to set secondary data, otherwise null</returns>
        Task<ISuccessOrErrors<TDto>> GetOriginalAsync(params object[] keys);
    }
}