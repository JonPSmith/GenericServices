﻿#region licence
// The MIT License (MIT)
// 
// Filename: UpdateServiceAsync.cs
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
using System.Threading.Tasks;
using GenericLibsBase;
using GenericLibsBase.Core;
using GenericServices.Core;
using GenericServices.Core.Internal;

namespace GenericServices.ServicesAsync.Concrete
{

    public class UpdateServiceAsync : IUpdateServiceAsync
    {
        private readonly IGenericServicesDbContext _db;

        public UpdateServiceAsync(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This updates the data in the database using the input data
        /// </summary>
        /// <typeparam name="T">The type of input data. 
        /// Type must be a type either an EF data class or one of the EfGenericDto's</typeparam>
        /// <param name="data">data to update the class. If Dto then copied over to data class</param>
        /// <returns></returns>
        public async Task<ISuccessOrErrors> UpdateAsync<T>(T data) where T : class
        {
            var service = DecodeToService<UpdateServiceAsync>.CreateCorrectService<T>(WhatItShouldBe.AsyncClassOrSpecificDto, _db);
            return await service.UpdateAsync(data).ConfigureAwait(false);
        }

        /// <summary>
        /// This is available to reset any secondary data in the dto. Call this if the ModelState was invalid and
        /// you need to display the view again with errors
        /// </summary>
        /// <param name="dto">Must be a dto inherited from EfGenericDtoAsync</param>
        /// <returns></returns>
        public async Task<T> ResetDtoAsync<T>(T dto) where T : class
        {
            var service = DecodeToService<UpdateServiceAsync>.CreateCorrectService<T>(WhatItShouldBe.AsyncSpecificDto, _db);
            return await service.ResetDtoAsync(dto).ConfigureAwait(false);
        }
    }

    //--------------------------------
    //direct

    public class UpdateServiceAsync<TEntity> : IUpdateServiceAsync<TEntity>
        where TEntity : class
    {
        private readonly IGenericServicesDbContext _db;


        public UpdateServiceAsync(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This updates the entity data class directly
        /// </summary>
        /// <param name="itemToUpdate"></param>
        /// <returns>status</returns>
        public async Task<ISuccessOrErrors> UpdateAsync(TEntity itemToUpdate)
        {
            if (itemToUpdate == null)
                throw new ArgumentNullException("itemToUpdate", "The item provided was null.");

            //Set the entry as modified
            _db.Entry(itemToUpdate).State = EntityState.Modified;

            var result = await _db.SaveChangesWithCheckingAsync().ConfigureAwait(false);
            if (result.IsValid)
                result.SetSuccessMessage("Successfully updated {0}.", typeof(TEntity).Name);

            return result;
        }
    }

    //------------------------------------------------------------------------
    //DTO version

    public class UpdateServiceAsync<TEntity, TDto> : IUpdateServiceAsync<TEntity,TDto>
        where TEntity : class, new()
        where TDto : EfGenericDtoAsync<TEntity, TDto>, new()
    {
        private readonly IGenericServicesDbContext _db;

        public UpdateServiceAsync(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This updates the entity data by copying over the relevant dto data.
        /// If it fails it resets the dto in case it is going to be shown again
        /// </summary>
        /// <param name="dto">If an error then its resets any secondary data so that you can reshow the dto</param>
        /// <returns>status</returns>
        public async Task<ISuccessOrErrors> UpdateAsync(TDto dto)
        {
            ISuccessOrErrors result = new SuccessOrErrors();
            if (!dto.SupportedFunctions.HasFlag(CrudFunctions.Update))
                return result.AddSingleError("Delete of a {0} is not supported in this mode.", dto.DataItemName);

            var itemToUpdate = await dto.FindItemTrackedForUpdateAsync(_db).ConfigureAwait(false);
            if (itemToUpdate == null)
                return result.AddSingleError("Could not find the {0} you requested.", dto.DataItemName);

            result = await dto.UpdateDataFromDtoAsync(_db, dto, itemToUpdate).ConfigureAwait(false); //update those properties we want to change
            if (result.IsValid)
            {
                result = await _db.SaveChangesWithCheckingAsync().ConfigureAwait(false);
                if (result.IsValid)
                    return result.SetSuccessMessage("Successfully updated {0}.", dto.DataItemName);
            }

            //otherwise there are errors
            if (!dto.SupportedFunctions.HasFlag(CrudFunctions.DoesNotNeedSetup))
                //we reset any secondary data as we expect the view to be reshown with the errors
                await dto.SetupSecondaryDataAsync(_db, dto).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// This is available to reset any secondary data in the dto. Call this if the ModelState was invalid and
        /// you need to display the view again with errors
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<TDto> ResetDtoAsync(TDto dto)
        {
            if (!dto.SupportedFunctions.HasFlag(CrudFunctions.DoesNotNeedSetup))
                //we reset any secondary data as we expect the view to be reshown with the errors
                await dto.SetupSecondaryDataAsync(_db, dto).ConfigureAwait(false);

            return dto;
        }
    }

}
