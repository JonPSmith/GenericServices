using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace GenericServices.Concrete
{
    public class DetailService<TData> : IDetailService<TData> where TData : class
    {
        private readonly IDbContextWithValidation _db;

        public DetailService(IDbContextWithValidation db)
        {
            _db = db;
        }

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Data class as read from database (not tracked)</returns>
        public TData GetDetail(Expression<Func<TData, bool>> whereExpression)
        {
            var result = _db.Set<TData>().Where(whereExpression).AsNoTracking().SingleOrDefault();
            if (result == null)
                throw new ArgumentException("We could not find an entry using the given predicate");
            return result;
        }
    }

    //---------------------------------------------------------------------

    public class DetailService<TData, TDto> : IDetailService<TData, TDto>
        where TData : class
        where TDto : EfGenericDto<TData, TDto>, new()
    {
        private readonly IDbContextWithValidation _db;

        public DetailService(IDbContextWithValidation db)
        {
            _db = db;
        }

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>TDto type with properties copyed over</returns>
        public TDto GetDetail(Expression<Func<TData, bool>> whereExpression)
        {
            var dto = new TDto();
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.Detail))
                throw new InvalidOperationException("This DTO does not support a detailed view.");

            return dto.CreateDtoAndCopyDataIn(_db, whereExpression);
        }
    }
}
