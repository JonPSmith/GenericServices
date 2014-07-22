using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericServices.Core;
using GenericServices.Core.Internal;

namespace GenericServices.ServicesAsync.Concrete
{

    public class UpdateSetupServiceAsync : IUpdateSetupServiceAsync
    {
        private readonly IDbContextWithValidation _db;

        public UpdateSetupServiceAsync(IDbContextWithValidation db)
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
        public async Task<T> GetOriginalAsync<T>(params object[] keys) where T : class
        {
            var service = DecodeToService<UpdateSetupServiceAsync>.CreateCorrectService<T>(WhatItShouldBe.AsyncClassOrSpecificDto, _db);
            return await service.GetOriginalAsync(keys);
        }
    }

    //--------------------------------
    //direct version

    public class UpdateSetupServiceAsync<TData> : IUpdateSetupServiceAsync<TData> where TData : class
    {
        private readonly IDbContextWithValidation _db;

        public UpdateSetupServiceAsync(IDbContextWithValidation db)
        {
            _db = db;
        }

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Data class as read from database (not tracked)</returns>
        public async Task<TData> GetOriginalUsingWhereAsync(Expression<Func<TData, bool>> whereExpression)
        {
            var result = await _db.Set<TData>().Where(whereExpression).AsNoTracking().SingleOrDefaultAsync();
            if (result == null)
                throw new ArgumentException("We could not find an entry using that filter. Has it been deleted by someone else?");
            return result;
        }

        /// <summary>
        /// This finds an entry using the primary key(s) in the data
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Data class as read from database (not tracked)</returns>
        public async Task<TData> GetOriginalAsync(params object[] keys)
        {
            return await GetOriginalUsingWhereAsync(BuildFilter.CreateFilter<TData>(_db.GetKeyProperties<TData>(), keys));
        }
    }

    //------------------------------------
    //Dto version



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
        public async Task<TDto> GetOriginalUsingWhereAsync(Expression<Func<TData, bool>> whereExpression)
        {
            var dto = new TDto();
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.Detail))
                throw new InvalidOperationException("This DTO does not support a detailed view.");

            dto = await dto.CreateDtoAndCopyDataInAsync(_db, whereExpression);
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                await dto.SetupSecondaryDataAsync(_db, dto);
            return dto;
        }


        /// <summary>
        /// This returns a single entry using the primary keys to find it. It also calls
        /// the dto's SetupSecondaryData to setup any extra data needed
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>TDto type with properties copyed over and SetupSecondaryData called to set secondary data</returns>
        public async Task<TDto> GetOriginalAsync(params object[] keys)
        {
            return await GetOriginalUsingWhereAsync(BuildFilter.CreateFilter<TData>(_db.GetKeyProperties<TData>(), keys));
        }
    }
}
