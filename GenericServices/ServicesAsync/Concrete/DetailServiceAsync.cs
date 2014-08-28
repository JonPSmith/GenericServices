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
        /// This returns a status which, if Valid, contains a single entry found using its primary keys.
        /// </summary>
        /// <typeparam name="T">The type of the data to output. 
        /// Type must be a type either an EF data class or one of the EfGenericDto's</typeparam>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Status. If valid Result holds data (not tracked), otherwise null</returns>
        public async Task<ISuccessOrErrors<T>> GetDetailAsync<T>(params object[] keys) where T : class, new()
        {
            var service = DecodeToService<DetailServiceAsync>.CreateCorrectService<T>(WhatItShouldBe.AsyncAnything, _db);
            return await service.GetDetailAsync(keys);
        }
    }

    //--------------------------------
    //direct

    public class DetailServiceAsync<TData> : IDetailServiceAsync<TData>
        where TData : class, new()
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
        /// <returns>Task with Status. If valid Result is data as read from database (not tracked), otherwise null</returns>
        public async Task<ISuccessOrErrors<TData>> GetDetailUsingWhereAsync(Expression<Func<TData, bool>> whereExpression)
        {
            return await _db.Set<TData>().Where(whereExpression).AsNoTracking().TrySingleWithPermissionCheckingAsync();
        }

        /// <summary>
        /// This finds an entry using the primary key(s) in the data
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Task with Status. If valid Result is data as read from database (not tracked), otherwise null</returns>
        public async Task<ISuccessOrErrors<TData>> GetDetailAsync(params object[] keys)
        {
            return await GetDetailUsingWhereAsync(BuildFilter.CreateFilter<TData>(_db.GetKeyProperties<TData>(), keys));
        }


    }

    //---------------------------------------------------------------------
    //DTO version

    public class DetailServiceAsync<TData, TDto> : IDetailServiceAsync<TData, TDto>
        where TData : class, new()
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
        /// <returns>Task with Status. If valid Result is data as read from database (not tracked), otherwise null</returns>
        public async Task<ISuccessOrErrors<TDto>> GetDetailUsingWhereAsync(Expression<Func<TData, bool>> whereExpression)
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
        /// <returns>Task with Status. If valid Result is data as read from database (not tracked), otherwise null</returns>
        public async Task<ISuccessOrErrors<TDto>> GetDetailAsync(params object[] keys)
        {
            return await GetDetailUsingWhereAsync(BuildFilter.CreateFilter<TData>(_db.GetKeyProperties<TData>(), keys));
        }
    }
}
