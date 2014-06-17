using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericServices.ServicesAsync;

namespace GenericServices
{
    public interface IDetailServiceAsync<TData>
        where TData : class
    {
        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Data class as read from database (not tracked)</returns>
        Task<TData> GetDetailAsync(Expression<Func<TData, bool>> whereExpression);
    }

    public interface IDetailServiceAsync<TData, TDto>
        where TData : class
        where TDto : EfGenericDtoAsync<TData, TDto>, new()
    {
        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>async Task TDto type with properties copyed over</returns>
        Task<TDto> GetDetailAsync(Expression<Func<TData, bool>> whereExpression);
    }
}