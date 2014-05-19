using System;
using System.Linq.Expressions;
using GenericServices.Concrete;

namespace GenericServices
{
    public interface IDetailService<TData> where TData : class
    {
        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Data class as read from database (not tracked)</returns>
        TData GetDetail(Expression<Func<TData, bool>> whereExpression);
    }

    public interface IDetailService<TData, out TDto>
        where TData : class
        where TDto : EfGenericDto<TData, TDto>, new()
    {
        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>TDto type with properties copyed over</returns>
        TDto GetDetail(Expression<Func<TData, bool>> whereExpression);
    }

}