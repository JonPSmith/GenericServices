using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using GenericServices.Core;
using GenericServices.Core.Internal;

namespace GenericServices.Services.Concrete
{

    public class UpdateSetupService : IUpdateSetupService
    {
        private readonly IDbContextWithValidation _db;

        public UpdateSetupService(IDbContextWithValidation db)
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
        public T GetOriginal<T>(params object[] keys) where T : class
        {
            var service = DecodeToService<UpdateSetupService>.CreateCorrectService<T>(WhatItShouldBe.SyncClassOrSpecificDto, _db);
            return service.GetOriginal(keys);
        }
    }

    //--------------------------------
    //direct version

    public class UpdateSetupService<TData> : IUpdateSetupService<TData> where TData : class
    {
        private readonly IDbContextWithValidation _db;

        public UpdateSetupService(IDbContextWithValidation db)
        {
            _db = db;
        }

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Data class as read from database (not tracked)</returns>
        public TData GetOriginalUsingWhere(Expression<Func<TData, bool>> whereExpression)
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
        public TData GetOriginal(params object[] keys)
        {
            return GetOriginalUsingWhere(BuildFilter.CreateFilter<TData>(_db.GetKeyProperties<TData>(), keys));
        }
    }

    //--------------------------------
    //dto version

    public class UpdateSetupService<TData, TDto> : IUpdateSetupService<TData, TDto> where TData : class
        where TDto : EfGenericDto<TData, TDto>, new()
    {
        private readonly IDbContextWithValidation _db;

        public UpdateSetupService(IDbContextWithValidation db)
        {
            _db = db;
        }

        /// <summary>
        /// This returns a single entry using the primary keys to find it. It also calls
        /// the dto's SetupSecondaryData to setup any extra data needed
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>TDto type with properties copyed over and SetupSecondaryData called to set secondary data</returns>
        public TDto GetOriginal(params object[] keys)
        {
            return GetOriginalUsingWhere(BuildFilter.CreateFilter<TData>(_db.GetKeyProperties<TData>(), keys));
        }

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part. It also calls
        /// the dto's SetupSecondaryData to setup any extra data needed
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>TDto type with properties copyed over and SetupSecondaryData called to set secondary data</returns>
        public TDto GetOriginalUsingWhere(Expression<Func<TData, bool>> whereExpression)
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
