using System;
using System.Linq.Expressions;
using GenericServices.Concrete;

namespace GenericServices
{
    internal interface IUpdateSetupService<TData, out TDto>
        where TData : class
        where TDto : EfGenericDto<TData, TDto>
    {
        /// <summary>
        /// This gets a single entry using the lambda expression as a where part. It also calls
        /// the dto's SetupSecondaryData to setup any extra data needed
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>TDto type with properties copyed over and SetupSecondaryData called to set secondary data</returns>
        TDto GetOriginal(Expression<Func<TData, bool>> whereExpression);
    }
}