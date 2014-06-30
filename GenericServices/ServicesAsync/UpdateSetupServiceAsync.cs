using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericServices.Core;

namespace GenericServices.ServicesAsync
{

    public class UpdateSetupServiceAsync<TData, TDto> : IUpdateSetupServiceAsync<TData, TDto> 
        where TData : class
        where TDto : EfGenericDtoAsync<TData, TDto>, new()
    {
        private readonly IDbContextWithValidation _db;

        public UpdateSetupServiceAsync(IDbContextWithValidation db)
        {
            _db = db;
        }

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part. It also calls
        /// the dto's SetupSecondaryData to setup any extra data needed
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>TDto type with properties copyed over and SetupSecondaryData called to set secondary data</returns>
        public async Task<TDto> GetOriginalAsync(Expression<Func<TData, bool>> whereExpression)
        {
            var dto = new TDto();
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.Detail))
                throw new InvalidOperationException("This DTO does not support a detailed view.");

            dto = await dto.CreateDtoAndCopyDataInAsync(_db, whereExpression);
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                await dto.SetupSecondaryDataAsync(_db, dto);
            return dto;
        }
    }
}
