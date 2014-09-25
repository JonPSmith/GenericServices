using System.Threading.Tasks;
using GenericServices.Core;
using GenericServices.Core.Internal;

namespace GenericServices.ServicesAsync.Concrete
{

    public class CreateSetupServiceAsync : ICreateSetupServiceAsync
    {
        private readonly IGenericServicesDbContext _db;

        public CreateSetupServiceAsync(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This returns the dto with any data that is needs for the view setup in it
        /// </summary>
        /// <typeparam name="TDto">The type of the data to output. This must be EfGeneric Dto</typeparam>
        /// <returns>The dto with any secondary data filled in</returns>
        public async Task<TDto> GetDtoAsync<TDto>() where TDto : class
        {
            var service = DecodeToService<CreateSetupServiceAsync>.CreateCorrectService<TDto>(WhatItShouldBe.AsyncSpecificDto, _db);
            return await service.GetDtoAsync();
        }
    }


    public class CreateSetupServiceAsync<TData, TDto> : ICreateSetupServiceAsync<TData, TDto>
        where TData : class, new()
        where TDto : EfGenericDtoAsync<TData, TDto>, new()
    {
                private readonly IGenericServicesDbContext _db;

        public CreateSetupServiceAsync(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This returns the dto with any data that is needs for the view setup in it
        /// </summary>
        /// <returns>A TDto which has had the SetupSecondaryData method called on it</returns>
        public async Task<TDto> GetDtoAsync()
        {
            var dto = new TDto();
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                await dto.SetupSecondaryDataAsync(_db, dto);

            return dto;
        }
    }
}
