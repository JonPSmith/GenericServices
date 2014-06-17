using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericServices.Services;

namespace GenericServices.ServicesAsync
{
    public class DetailServiceAsync<TData> : IDetailServiceAsync<TData>
        where TData : class
    {
        private readonly IDbContextWithValidation _db;

        public DetailServiceAsync(IDbContextWithValidation db)
        {
            _db = db;
        }

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Data class as read from database (not tracked)</returns>
        public async Task<TData> GetDetailAsync(Expression<Func<TData, bool>> whereExpression)
        {
            var result = await _db.Set<TData>().Where(whereExpression).AsNoTracking().SingleOrDefaultAsync();
            if (result == null)
                throw new ArgumentException("We could not find an entry using the given predicate");
            return result;
        }
    }

    //---------------------------------------------------------------------

    public class DetailServiceAsync<TData, TDto> : IDetailServiceAsync<TData, TDto> 
        where TData : class
        where TDto : EfGenericDtoAsync<TData, TDto>, new()
    {
        private readonly IDbContextWithValidation _db;

        public DetailServiceAsync(IDbContextWithValidation db)
        {
            _db = db;
        }

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>async Task TDto type with properties copyed over</returns>
        public async Task<TDto> GetDetailAsync(Expression<Func<TData, bool>> whereExpression)
        {
            var dto = new TDto();
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.Detail))
                throw new InvalidOperationException("This DTO does not support a detailed view.");

            return await dto.CreateDtoAndCopyDataInAsync(_db, whereExpression);
        }
    }
}
