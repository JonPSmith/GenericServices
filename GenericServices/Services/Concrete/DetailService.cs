#region licence
// The MIT License (MIT)
// 
// Filename: DetailService.cs
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
using GenericLibsBase;
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

            return dto.DetailDtoFromDataIn(_db, whereExpression);
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
