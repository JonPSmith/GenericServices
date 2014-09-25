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
        private readonly IGenericServicesDbContext _db;

        public UpdateSetupService(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This returns a status which, if Valid, has single entry using the primary keys to find it.
        /// </summary>
        /// <typeparam name="T">The type of the data to output. 
        /// Type must be a type either an EF data class or one of the EfGenericDto's</typeparam>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Status. If valid Result holds data (not tracked), otherwise null</returns>
        public ISuccessOrErrors<T> GetOriginal<T>(params object[] keys) where T : class
        {
            var service = DecodeToService<UpdateSetupService>.CreateCorrectService<T>(WhatItShouldBe.SyncClassOrSpecificDto, _db);
            return service.GetOriginal(keys);
        }
    }

    //--------------------------------
    //direct version

    public class UpdateSetupService<TData> : IUpdateSetupService<TData> where TData : class, new()
    {
        private readonly IGenericServicesDbContext _db;

        public UpdateSetupService(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Status. If valid Result holds data (not tracked), otherwise null</returns>
        public ISuccessOrErrors<TData> GetOriginalUsingWhere(Expression<Func<TData, bool>> whereExpression)
        {
            return _db.Set<TData>().Where(whereExpression).AsNoTracking().RealiseSingleWithErrorChecking();
        }

        /// <summary>
        /// This finds an entry using the primary key(s) in the data
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Status. If valid Result holds data (not tracked), otherwise null</returns>
        public ISuccessOrErrors<TData> GetOriginal(params object[] keys)
        {
            return GetOriginalUsingWhere(BuildFilter.CreateFilter<TData>(_db.GetKeyProperties<TData>(), keys));
        }
    }

    //--------------------------------
    //dto version

    public class UpdateSetupService<TData, TDto> : IUpdateSetupService<TData, TDto>
        where TData : class, new()
        where TDto : EfGenericDto<TData, TDto>, new()
    {
        private readonly IGenericServicesDbContext _db;

        public UpdateSetupService(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This returns a single entry using the primary keys to find it. It also calls
        /// the dto's SetupSecondaryData to setup any extra data needed
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Status. If valid TDto type with properties copyed over and SetupSecondaryData called 
        /// to set secondary data, otherwise null</returns>
        public ISuccessOrErrors<TDto> GetOriginal(params object[] keys)
        {
            return GetOriginalUsingWhere(BuildFilter.CreateFilter<TData>(_db.GetKeyProperties<TData>(), keys));
        }

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part. It also calls
        /// the dto's SetupSecondaryData to setup any extra data needed
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Status. If valid TDto type with properties copyed over and SetupSecondaryData called 
        /// to set secondary data, otherwise null</returns>
        public ISuccessOrErrors<TDto> GetOriginalUsingWhere(Expression<Func<TData, bool>> whereExpression)
        {
            var dto = new TDto();
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.Detail))
                throw new InvalidOperationException("This DTO does not support a detailed view.");

            var status = dto.CreateDtoAndCopyDataIn(_db, whereExpression);
            if (!status.IsValid) return status;

            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                status.Result.SetupSecondaryData(_db, status.Result);
            return status;
        }
    }
}
