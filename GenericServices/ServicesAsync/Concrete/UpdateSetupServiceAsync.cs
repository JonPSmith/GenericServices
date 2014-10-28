#region licence
// The MIT License (MIT)
// 
// Filename: UpdateSetupServiceAsync.cs
// Date Created: 2014/07/22
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericLibsBase;
using GenericServices.Core;
using GenericServices.Core.Internal;

namespace GenericServices.ServicesAsync.Concrete
{

    public class UpdateSetupServiceAsync : IUpdateSetupServiceAsync
    {
        private readonly IGenericServicesDbContext _db;

        public UpdateSetupServiceAsync(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This returns a status which, if Valid, has single entry using the primary keys to find it.
        /// </summary>
        /// <typeparam name="T">The type of the data to output. 
        /// Type must be a type either an EF data class or one of the EfGenericDto's</typeparam>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Task with Status. If valid Result holds data (not tracked), otherwise null</returns>
        public async Task<ISuccessOrErrors<T>> GetOriginalAsync<T>(params object[] keys) where T : class
        {
            var service = DecodeToService<UpdateSetupServiceAsync>.CreateCorrectService<T>(WhatItShouldBe.AsyncClassOrSpecificDto, _db);
            return await service.GetOriginalAsync(keys);
        }
    }

    //--------------------------------
    //direct version

    public class UpdateSetupServiceAsync<TEntity> : IUpdateSetupServiceAsync<TEntity> where TEntity : class, new()
    {
        private readonly IGenericServicesDbContext _db;

        public UpdateSetupServiceAsync(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Task with Status. If valid Result holds data (not tracked), otherwise null</returns>
        public async Task<ISuccessOrErrors<TEntity>> GetOriginalUsingWhereAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await _db.Set<TEntity>().Where(whereExpression).AsNoTracking().RealiseSingleWithErrorCheckingAsync();
        }

        /// <summary>
        /// This finds an entry using the primary key(s) in the data
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Task with Status. If valid Result holds data (not tracked), otherwise null</returns>
        public async Task<ISuccessOrErrors<TEntity>> GetOriginalAsync(params object[] keys)
        {
            return await GetOriginalUsingWhereAsync(BuildFilter.CreateFilter<TEntity>(_db.GetKeyProperties<TEntity>(), keys));
        }
    }

    //------------------------------------
    //Dto version

    public class UpdateSetupServiceAsync<TEntity, TDto> : IUpdateSetupServiceAsync<TEntity, TDto>
        where TEntity : class, new()
        where TDto : EfGenericDtoAsync<TEntity, TDto>, new()
    {
        private readonly IGenericServicesDbContext _db;

        public UpdateSetupServiceAsync(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part. It also calls
        /// the dto's SetupSecondaryData to setup any extra data needed
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Task with Status. If valid TDto type with properties copyed over and SetupSecondaryData called 
        /// to set secondary data, otherwise null</returns>
        public async Task<ISuccessOrErrors<TDto>> GetOriginalUsingWhereAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            var dto = new TDto();
            if (!dto.SupportedFunctions.HasFlag(CrudFunctions.Update))
                throw new InvalidOperationException("This DTO does not support update.");

            var status = await dto.DetailDtoFromDataInAsync(_db, whereExpression);
            if (!status.IsValid) return status;

            if (!dto.SupportedFunctions.HasFlag(CrudFunctions.DoesNotNeedSetup))
                await status.Result.SetupSecondaryDataAsync(_db, status.Result);
            return status;
        }

        /// <summary>
        /// This returns a single entry using the primary keys to find it. It also calls
        /// the dto's SetupSecondaryData to setup any extra data needed
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Task with Status. If valid TDto type with properties copyed over and SetupSecondaryData called 
        /// to set secondary data, otherwise null</returns>
        public async Task<ISuccessOrErrors<TDto>> GetOriginalAsync(params object[] keys)
        {
            return await GetOriginalUsingWhereAsync(BuildFilter.CreateFilter<TEntity>(_db.GetKeyProperties<TEntity>(), keys));
        }
    }
}
