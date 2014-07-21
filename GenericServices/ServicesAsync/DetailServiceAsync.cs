using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericServices.Core;
using GenericServices.Core.Internal;
using GenericServices.Services;

namespace GenericServices.ServicesAsync
{

    public class DetailServiceAsync : IDetailServiceAsync
    {
        private readonly IDbContextWithValidation _db;

        public DetailServiceAsync(IDbContextWithValidation db)
        {
            _db = db;
        }

        /// <summary>
        /// This works out what sort of service is needed from the type provided
        /// and returns a single entry using the lambda expression as a where part
        /// </summary>
        /// <typeparam name="T">The type of the data to output. 
        /// Type must be a type either an EF data class or a class based on EfGenericDtoAsync</typeparam>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Data class as read from database (not tracked)</returns>
        public T GetDetail<T>(Expression<Func<T, bool>> whereExpression) where T : class
        {
            var service = DecodeToService<DetailServiceAsync>.CreateCorrectService<T>(WhatItShouldBe.AsyncAnything, _db);
            return service.GetDetail(whereExpression);
        }
    }

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
        public async Task<TDto> GetDetailAsync(Expression<Func<TData, bool>> whereExpression)
        {
            var dto = new TDto();
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.Detail))
                throw new InvalidOperationException("This DTO does not support a detailed view.");

            return await dto.CreateDtoAndCopyDataInAsync(_db, whereExpression);
        }
    }
}
