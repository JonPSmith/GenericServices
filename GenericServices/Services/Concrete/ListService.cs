#region licence
// The MIT License (MIT)
// 
// Filename: ListService.cs
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
using System.Linq;
using GenericServices.Core;
using GenericServices.Core.Internal;

namespace GenericServices.Services.Concrete
{

    public class ListService : IListService
    {
        private readonly IGenericServicesDbContext _db;

        public ListService(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This returns an IQueryable list of all items of the given type
        /// </summary>
        /// <typeparam name="T">The type of the data to output. 
        /// Type must be a type either an EF data class or a class inherited from the EfGenericDto or EfGenericDtoAsync</typeparam>
        /// <returns>note: the list items are not tracked</returns>
        public IQueryable<T> GetAll<T>() where T : class, new()
        {
            var service = DecodeToService<ListService>.CreateCorrectService<T>(WhatItShouldBe.SyncAnything, _db);
            return service.GetAll();
        }
    }


    public class ListService<TData> : IListService<TData> where TData : class
    {
        private readonly IGenericServicesDbContext _db;

        public ListService(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This returns an IQueryable list of all items of the given type
        /// </summary>
        /// <returns>note: the list items are not tracked</returns>
        public IQueryable<TData> GetAll()
        {
            return _db.Set<TData>().AsNoTracking();
        }

    }

    //---------------------------------------------------------------------------
    //DTO version

    public class ListService<TData, TDto> : IListService<TData, TDto>
        where TData : class
        where TDto : EfGenericDtoBase<TData, TDto>, new()
    {
        private readonly IGenericServicesDbContext _db;

        public ListService(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This returns an IQueryable list of all items of the given TData, but transformed into TDto data type
        /// </summary>
        /// <returns>note: the list items are not tracked</returns>
        public IQueryable<TDto> GetAll()
        {
            var tDto = new TDto();
            if (!tDto.SupportedFunctions.HasFlag(CrudFunctions.List))
                throw new InvalidOperationException("This DTO does not support listings.");

            return tDto.ListQueryUntracked(_db);
        }
    }

}
