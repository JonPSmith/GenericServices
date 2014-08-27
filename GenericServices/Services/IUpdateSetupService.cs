using System;
using System.Linq.Expressions;
using GenericServices.Core;

namespace GenericServices.Services
{
    
    public interface IUpdateSetupService<TData> where TData : class
    {
        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Status. If valid Result holds data (not tracked), otherwise null</returns>
        ISuccessOrErrors<TData> GetOriginalUsingWhere(Expression<Func<TData, bool>> whereExpression);

        /// <summary>
        /// This finds an entry using the primary key(s) in the data
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Status. If valid Result holds data (not tracked), otherwise null</returns>
        ISuccessOrErrors<TData> GetOriginal(params object[] keys);
    }


    public interface IUpdateSetupService<TData, TDto>
        where TData : class
        where TDto : EfGenericDto<TData, TDto>
    {
        /// <summary>
        /// This returns a single entry using the primary keys to find it. It also calls
        /// the dto's SetupSecondaryData to setup any extra data needed
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Status. If valid TDto type with properties copyed over and SetupSecondaryData called 
        /// to set secondary data, otherwise null</returns>
        ISuccessOrErrors<TDto> GetOriginal(params object[] keys);

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part. It also calls
        /// the dto's SetupSecondaryData to setup any extra data needed
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>TDto type with properties copyed over and SetupSecondaryData called to set secondary data</returns>
        ISuccessOrErrors<TDto> GetOriginalUsingWhere(Expression<Func<TData, bool>> whereExpression);
    }
}