using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericServices.Core;
using GenericServices.Core.Internal;

namespace GenericServices.ServicesAsync.Concrete
{

    public class DetailServiceAsync : IDetailServiceAsync
    {
        private readonly IDbContextWithValidation _db;

        public DetailServiceAsync(IDbContextWithValidation db)
        {
            _db = db;
        }

        /// <summary>
        /// This returns a single entry using the primary keys to find it.
        /// </summary>
        /// <typeparam name="T">The type of the data to output. 
        /// Type must be a type either an EF data class or one of the EfGenericDto's</typeparam>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Data class as read from database (not tracked)</returns>
        public async Task<T> GetDetailAsync<T>(params object[] keys) where T : class, new()
        {
            var service = DecodeToService<DetailServiceAsync>.CreateCorrectService<T>(WhatItShouldBe.AsyncAnything, _db);
            return await service.GetDetailAsync(keys);
        }
    }

    //--------------------------------
    //direct

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
        public async Task<TData> GetDetailUsingWhereAsync(Expression<Func<TData, bool>> whereExpression)
        {
            var result = await _db.Set<TData>().Where(whereExpression).AsNoTracking().SingleOrDefaultAsync();
            if (result == null)
                throw new ArgumentException("We could not find an entry using the given predicate");
            return result;
        }

        /// <summary>
        /// This finds an entry using the primary key(s) in the data
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Data class as read from database (not tracked)</returns>
        public async Task<TData> GetDetailAsync(params object[] keys)
        {
            return await GetDetailUsingWhereAsync(BuildFilter.CreateFilter<TData>(_db.GetKeyProperties<TData>(), keys));
        }


    }

    //---------------------------------------------------------------------
    //DTO version

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
        public async Task<TDto> GetDetailUsingWhereAsync(Expression<Func<TData, bool>> whereExpression)
        {
            var dto = new TDto();
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.Detail))
                throw new InvalidOperationException("This DTO does not support a detailed view.");

            return await dto.CreateDtoAndCopyDataInAsync(_db, whereExpression);
        }

        /// <summary>
        /// This finds an entry using the primary key(s) in the data
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>TDto type with properties copied over</returns>
        public async Task<TDto> GetDetailAsync(params object[] keys)
        {
            return await GetDetailUsingWhereAsync(BuildFilter.CreateFilter<TData>(_db.GetKeyProperties<TData>(), keys));
        }
    }
}
