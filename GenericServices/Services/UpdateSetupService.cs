using System;
using System.Linq.Expressions;

namespace GenericServices.Services
{
    public class UpdateSetupService<TData, TDto> : IUpdateSetupService<TData, TDto> where TData : class
        where TDto : EfGenericDto<TData, TDto>, new()
    {
        private readonly IDbContextWithValidation _db;

        public UpdateSetupService(IDbContextWithValidation db)
        {
            _db = db;
        }

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part. It also calls
        /// the dto's SetupSecondaryData to setup any extra data needed
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>TDto type with properties copyed over and SetupSecondaryData called to set secondary data</returns>
        public TDto GetOriginal(Expression<Func<TData, bool>> whereExpression)
        {
            var dto = new TDto();
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.Detail))
                throw new InvalidOperationException("This DTO does not support a detailed view.");

            dto = dto.CreateDtoAndCopyDataIn(_db, whereExpression);
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                dto.SetupSecondaryData(_db, dto);
            return dto;
        }
    }
}
