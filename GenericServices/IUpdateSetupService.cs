using System;
using System.Linq.Expressions;
using GenericServices.Core;

namespace GenericServices
{
    public interface IUpdateSetupService
    {
        /// <summary>
        /// This returns a single entry using the primary keys to find it.
        /// </summary>
        /// <typeparam name="T">The type of the data to output. 
        /// Type must be a type either an EF data class or one of the EfGenericDto's</typeparam>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Data class as read from database (not tracked)</returns>
        T GetOriginal<T>(params object[] keys) where T : class;
    }

    public interface IUpdateSetupService<TData> where TData : class
    {
        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Data class as read from database (not tracked)</returns>
        TData GetOriginalUsingWhere(Expression<Func<TData, bool>> whereExpression);

        /// <summary>
        /// This finds an entry using the primary key(s) in the data
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Data class as read from database (not tracked)</returns>
        TData GetOriginal(params object[] keys);
    }


    public interface IUpdateSetupService<TData, out TDto>
        where TData : class
        where TDto : EfGenericDto<TData, TDto>
    {
        /// <summary>
        /// This returns a single entry using the primary keys to find it. It also calls
        /// the dto's SetupSecondaryData to setup any extra data needed
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>TDto type with properties copyed over and SetupSecondaryData called to set secondary data</returns>
        TDto GetOriginal(params object[] keys);

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part. It also calls
        /// the dto's SetupSecondaryData to setup any extra data needed
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>TDto type with properties copyed over and SetupSecondaryData called to set secondary data</returns>
        TDto GetOriginalUsingWhere(Expression<Func<TData, bool>> whereExpression);
    }
}