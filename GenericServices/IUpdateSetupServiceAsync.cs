using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericServices.ServicesAsync;

namespace GenericServices
{
    public interface IUpdateSetupServiceAsync<TData, TDto>
        where TData : class
        where TDto : EfGenericDtoAsync<TData, TDto>, new()
    {
        /// <summary>
        /// This gets a single entry using the lambda expression as a where part. It also calls
        /// the dto's SetupSecondaryData to setup any extra data needed
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>TDto type with properties copyed over and SetupSecondaryData called to set secondary data</returns>
        Task<TDto> GetOriginalAsync(Expression<Func<TData, bool>> whereExpression);
    }
}