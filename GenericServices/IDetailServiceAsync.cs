using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericServices.Core;

namespace GenericServices
{
    public interface IDetailServiceAsync
    {
        /// <summary>
        /// This works out what sort of service is needed from the type provided
        /// and returns a single entry using the lambda expression as a where part
        /// </summary>
        /// <typeparam name="T">The type of the data to output. 
        /// Type must be a type either an EF data class or a class based on EfGenericDtoAsync</typeparam>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Data class as read from database (not tracked)</returns>
        T GetDetail<T>(Expression<Func<T, bool>> whereExpression) where T : class;
    }

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