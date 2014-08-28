using System;
using System.Linq.Expressions;
using GenericServices.Core;

namespace GenericServices.Services
{

    public interface IDetailService<TData> where TData : class, new()
    {
        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Status. If valid Result is data as read from database (not tracked), otherwise null</returns>
        ISuccessOrErrors<TData> GetDetailUsingWhere(Expression<Func<TData, bool>> whereExpression);

        /// <summary>
        /// This finds an entry using the primary key(s) in the data
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Status. If valid Result is data as read from database (not tracked), otherwise null</returns>
        ISuccessOrErrors<TData> GetDetail(params object[] keys);
    }

    public interface IDetailService<TData, TDto>
        where TData : class, new()
        where TDto : EfGenericDto<TData, TDto>, new()
    {
        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Status. If Valid then TDto type with properties copyed over, else null</returns>
        ISuccessOrErrors<TDto> GetDetailUsingWhere(Expression<Func<TData, bool>> whereExpression);

        /// <summary>
        /// This finds an entry using the primary key(s) in the data
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Status. If Valid then TDto type with properties copyed over, else null</returns>
        ISuccessOrErrors<TDto> GetDetail(params object[] keys);
    }

}