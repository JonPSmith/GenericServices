using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using GenericServices.Core;
using GenericServices.Core.Internal;

namespace GenericServices.Services.Concrete
{

    public class DetailService : IDetailService
    {
        private readonly IGenericServicesDbContext _db;

        public DetailService(IGenericServicesDbContext db)
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
        public ISuccessOrErrors<T> GetDetail<T>(params object[] keys) where T : class, new()
        {
            var service = DecodeToService<DetailService>.CreateCorrectService<T>(WhatItShouldBe.SyncAnything, _db);
            return service.GetDetail(keys);
        }
    }

    //--------------------------------
    //direct version

    public class DetailService<TData> : IDetailService<TData> where TData : class, new()
    {
        private readonly IGenericServicesDbContext _db;

        public DetailService(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part. Checks for problems
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Status. If valid Result is data as read from database (not tracked), otherwise null</returns>
        public ISuccessOrErrors<TData> GetDetailUsingWhere(Expression<Func<TData, bool>> whereExpression)
        {
            return _db.Set<TData>().Where(whereExpression).AsNoTracking().RealiseSingleWithErrorChecking();
        }

        /// <summary>
        /// This finds an entry using the primary key(s) in the data
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Status. If valid Result is data as read from database (not tracked), otherwise null</returns>
        public ISuccessOrErrors<TData> GetDetail(params object[] keys)
        {
            return GetDetailUsingWhere(BuildFilter.CreateFilter<TData>(_db.GetKeyProperties<TData>(), keys));
        }
    }

    //---------------------------------------------------------------------
    //DTO version

    public class DetailService<TData, TDto> : IDetailService<TData, TDto>
        where TData : class, new()
        where TDto : EfGenericDto<TData, TDto>, new()
    {
        private readonly IGenericServicesDbContext _db;

        public DetailService(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Status. If Valid then TDto type with properties copyed over, else null</returns>
        public ISuccessOrErrors<TDto> GetDetailUsingWhere(Expression<Func<TData, bool>> whereExpression)
        {
            var dto = new TDto();
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.Detail))
                throw new InvalidOperationException("This DTO does not support a detailed view.");

            return dto.CreateDtoAndCopyDataIn(_db, whereExpression);
        }

        /// <summary>
        /// This finds an entry using the primary key(s) in the data
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Status. If Valid then TDto type with properties copyed over, else null</returns>
        public ISuccessOrErrors<TDto> GetDetail(params object[] keys)
        {
            return GetDetailUsingWhere(BuildFilter.CreateFilter<TData>(_db.GetKeyProperties<TData>(), keys));
        }
    }
}
