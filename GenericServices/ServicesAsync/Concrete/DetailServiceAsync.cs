#region licence
// The MIT License (MIT)
// 
// Filename: DetailServiceAsync.cs
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

    public class DetailServiceAsync : IDetailServiceAsync
    {
        private readonly IGenericServicesDbContext _db;

        public DetailServiceAsync(IGenericServicesDbContext db)
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

    public class DetailServiceAsync<TEntity> : IDetailServiceAsync<TEntity>
        where TEntity : class, new()
    {
        private readonly IGenericServicesDbContext _db;

        public DetailServiceAsync(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Task with Status. If valid Result is data as read from database (not tracked), otherwise null</returns>
        public async Task<ISuccessOrErrors<TEntity>> GetDetailUsingWhereAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await _db.Set<TEntity>().Where(whereExpression).AsNoTracking().RealiseSingleWithErrorCheckingAsync();
        }

        /// <summary>
        /// This finds an entry using the primary key(s) in the data
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Task with Status. If valid Result is data as read from database (not tracked), otherwise null</returns>
        public async Task<ISuccessOrErrors<TEntity>> GetDetailAsync(params object[] keys)
        {
            return await GetDetailUsingWhereAsync(BuildFilter.CreateFilter<TEntity>(_db.GetKeyProperties<TEntity>(), keys));
        }


    }

    //---------------------------------------------------------------------
    //DTO version

    public class DetailServiceAsync<TEntity, TDto> : IDetailServiceAsync<TEntity, TDto>
        where TEntity : class, new()
        where TDto : EfGenericDtoAsync<TEntity, TDto>, new()
    {
        private readonly IGenericServicesDbContext _db;

        public DetailServiceAsync(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This gets a single entry using the lambda expression as a where part
        /// </summary>
        /// <param name="whereExpression">Should be a 'where' expression that returns one item</param>
        /// <returns>Task with Status. If valid Result is data as read from database (not tracked), otherwise null</returns>
        public async Task<ISuccessOrErrors<TDto>> GetDetailUsingWhereAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            var dto = new TDto();
            if (!dto.SupportedFunctions.HasFlag(CrudFunctions.Detail))
                throw new InvalidOperationException("This DTO does not support a detailed view.");

            return await dto.DetailDtoFromDataInAsync(_db, whereExpression);
        }

        /// <summary>
        /// This finds an entry using the primary key(s) in the data
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns>Task with Status. If valid Result is data as read from database (not tracked), otherwise null</returns>
        public async Task<ISuccessOrErrors<TDto>> GetDetailAsync(params object[] keys)
        {
            return await GetDetailUsingWhereAsync(BuildFilter.CreateFilter<TEntity>(_db.GetKeyProperties<TEntity>(), keys));
        }
    }
}
