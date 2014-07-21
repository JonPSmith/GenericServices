using System;
using System.Data.Entity;
using System.Threading.Tasks;
using GenericServices.Core;
using GenericServices.Core.Internal;

namespace GenericServices.ServicesAsync
{

    public class UpdateServiceAsync : IUpdateServiceAsync
    {
        private readonly IDbContextWithValidation _db;

        public UpdateServiceAsync(IDbContextWithValidation db)
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
            return await service.UpdateAsync(data);
        }
    }

    //--------------------------------
    //direct

    public class UpdateServiceAsync<TData> : IUpdateServiceAsync<TData>
        where TData : class
    {
        private readonly IDbContextWithValidation _db;


        public UpdateServiceAsync(IDbContextWithValidation db)
        {
            _db = db;
        }

        /// <summary>
        /// This updates the entity data class directly
        /// </summary>
        /// <param name="itemToUpdate"></param>
        /// <returns>status</returns>
        public async Task<ISuccessOrErrors> UpdateAsync(TData itemToUpdate)
        {
            if (itemToUpdate == null)
                throw new ArgumentNullException("itemToUpdate", "The item provided was null.");

            //Set the entry as modified
            _db.Entry(itemToUpdate).State = EntityState.Modified;

            var result = await _db.SaveChangesWithValidationAsync();
            if (result.IsValid)
                result.SetSuccessMessage("Successfully updated {0}.", typeof(TData).Name);

            return result;
        }
    }

    //------------------------------------------------------------------------
    //DTO version

    public class UpdateServiceAsync<TData, TDto> : IUpdateServiceAsync<TData,TDto>
        where TData : class
        where TDto : EfGenericDtoAsync<TData, TDto>
    {
        private readonly IDbContextWithValidation _db;

        public UpdateServiceAsync(IDbContextWithValidation db)
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
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.Update))
                return result.AddSingleError("Delete of a {0} is not supported in this mode.", dto.DataItemName);

            var itemToUpdate = await dto.FindItemTrackedAsync(_db);
            if (itemToUpdate == null)
                return result.AddSingleError("Could not find the {0} you requested.", dto.DataItemName);

            result = await dto.CopyDtoToDataAsync(_db, dto, itemToUpdate); //update those properties we want to change
            if (result.IsValid)
            {
                result = await _db.SaveChangesWithValidationAsync();
                if (result.IsValid)
                    return result.SetSuccessMessage("Successfully updated {0}.", dto.DataItemName);
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
