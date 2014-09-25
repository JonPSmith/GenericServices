﻿using System;
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
            if (!tDto.SupportedFunctions.HasFlag(ServiceFunctions.List))
                throw new InvalidOperationException("This DTO does not support listings.");

            return tDto.BuildListQueryUntracked(_db);
        }
    }

}
