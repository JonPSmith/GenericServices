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
        private readonly IDbContextWithValidation _db;

        public DetailService(IDbContextWithValidation db)
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
        public T GetDetail<T>(params object[] keys) where T : class, new()
        {
            var service = DecodeToService<DetailService>.CreateCorrectService<T>(WhatItShouldBe.SyncAnything, _db);
            return service.GetDetail(keys);
        }
    }

    //--------------------------------
    //direct version

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
        public TData GetDetailUsingWhere(Expression<Func<TData, bool>> whereExpression)
        {
            var result = _db.Set<TData>().Where(whereExpression).AsNoTracking().SingleOrDefault();
            if (result == null)
                throw new ArgumentException("We could not find an entry using that filter. Has it been deleted by someone else?");
            return result;
        }

        /// <summary>
        /// This finds an entry using the primary key(s) in the data
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Data class as read from database (not tracked)</returns>
        public TData GetDetail(params object[] keys)
        {
            return GetDetailUsingWhere(BuildFilter.CreateFilter<TData>(_db.GetKeyProperties<TData>(), keys));
        }
    }

    //---------------------------------------------------------------------
    //DTO version

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
        public TDto GetDetailUsingWhere(Expression<Func<TData, bool>> whereExpression)
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
        /// <returns>TDto type with properties copied over</returns>
        public TDto GetDetail(params object[] keys)
        {
            return GetDetailUsingWhere(BuildFilter.CreateFilter<TData>(_db.GetKeyProperties<TData>(), keys));
        }
    }
}
