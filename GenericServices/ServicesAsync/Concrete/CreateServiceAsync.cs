#region licence
// The MIT License (MIT)
// 
// Filename: CreateServiceAsync.cs
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

using System.Threading.Tasks;
using GenericLibsBase;
using GenericLibsBase.Core;
using GenericServices.Core;
using GenericServices.Core.Internal;

namespace GenericServices.ServicesAsync.Concrete
{


    public class CreateServiceAsync : ICreateServiceAsync
    {
        private readonly IGenericServicesDbContext _db;

        public CreateServiceAsync(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This adds a new entity class to the database with error checking
        /// </summary>
        /// <typeparam name="T">The type of the data to output. 
        /// Type must be a type either an EF data class or one of the EfGenericDto's</typeparam>
        /// <param name="newItem">either entity class or dto to create the data item with</param>
        /// <returns>status</returns>
        public async Task<ISuccessOrErrors> CreateAsync<T>(T newItem) where T : class
        {
            var service = DecodeToService<CreateServiceAsync>.CreateCorrectService<T>(WhatItShouldBe.AsyncAnything, _db);
            return await service.CreateAsync(newItem);
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
            return await service.ResetDtoAsync(dto);
        }
    }

    //-----------------------------
    //direct

    public class CreateServiceAsync<TData> : ICreateServiceAsync<TData> where TData : class
    {
        private readonly IGenericServicesDbContext _db;

        public CreateServiceAsync(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This adds a new entity class to the database with error checking
        /// </summary>
        /// <param name="newItem"></param>
        /// <returns>status</returns>
        public async Task<ISuccessOrErrors> CreateAsync(TData newItem)
        {
            _db.Set<TData>().Add(newItem);
            var result = await _db.SaveChangesWithCheckingAsync();
            if (result.IsValid)
                result.SetSuccessMessage("Successfully created {0}.", typeof(TData).Name);

            return result;
        }

    }

    //---------------------------------------------------------------------------
    //DTO version

    public class CreateServiceAsync<TData, TDto> : ICreateServiceAsync<TData, TDto> 
        where TData : class, new()
        where TDto : EfGenericDtoAsync<TData, TDto>, new()
    {
        private readonly IGenericServicesDbContext _db;


        public CreateServiceAsync(IGenericServicesDbContext db)
        {
            _db = db;
        }


        /// <summary>
        /// This uses a dto to create a data class which it writes to the database with error checking
        /// </summary>
        /// <param name="dto">If an error then its resets any secondary data so that you can reshow the dto</param>
        /// <returns>status</returns>
        public async Task<ISuccessOrErrors> CreateAsync(TDto dto)
        {
            ISuccessOrErrors result = new SuccessOrErrors();
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.Create))
                return result.AddSingleError("Create of a new {0} is not supported in this mode.", dto.DataItemName);

            var statusWithData = await dto.CreateDataFromDtoAsync(_db, dto);    //creates the new data and fills in the properties
            result = statusWithData as ISuccessOrErrors;             //convert to normal status as need errors to fall through propertly
            if (result.IsValid)
            {
                _db.Set<TData>().Add(statusWithData.Result);
                result = await _db.SaveChangesWithCheckingAsync();
                if (result.IsValid)
                    return result.SetSuccessMessage("Successfully created {0}.", dto.DataItemName);
            }

            //otherwise there are errors
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                //we reset any secondary data as we expect the view to be reshown with the errors
                await dto.SetupSecondaryDataAsync(_db, dto);
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
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                //we reset any secondary data as we expect the view to be reshown with the errors
                await dto.SetupSecondaryDataAsync(_db, dto);

            return dto;
        }

    }
}
