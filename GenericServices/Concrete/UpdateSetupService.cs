using System;
using System.Linq.Expressions;

namespace GenericServices.Concrete
{
    public class UpdateSetupService<TData, TDto> : IUpdateSetupService<TData, TDto> where TData : class
        where TDto : EfGenericDto<TData, TDto>
    {
        private readonly IDbContextWithValidation _db;
        private TDto _tDto;

        public UpdateSetupService(IDbContextWithValidation db, TDto tDto)
        {
            _db = db;
            _tDto = tDto;
        }

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part. It also calls
        /// the dto's SetupSecondaryData to setup any extra data needed
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>TDto type with properties copyed over and SetupSecondaryData called to set secondary data</returns>
        public TDto GetOriginal(Expression<Func<TData, bool>> whereExpression)
        {
            if (!_tDto.SupportedFunctions.HasFlag(ServiceFunctions.Detail))
                throw new InvalidOperationException("This DTO does not support a detailed view.");

            _tDto = _tDto.CreateDtoAndCopyDataIn(_db, whereExpression);
            _tDto.SetupSecondaryData(_db, _tDto);
            return _tDto;
        }
    }
}
