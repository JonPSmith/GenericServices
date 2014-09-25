using GenericServices.Core;
using GenericServices.Core.Internal;

namespace GenericServices.Services.Concrete
{

    public class CreateSetupService : ICreateSetupService
    {

        private readonly IGenericServicesDbContext _db;

        public CreateSetupService(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This returns the dto with any data that is needs for the view setup in it
        /// </summary>
        /// <typeparam name="TDto">The type of the data to output. This must be EfGeneric Dto</typeparam>
        /// <returns>The dto with any secondary data filled in</returns>
        public TDto GetDto<TDto>() where TDto : class
        {
            var service = DecodeToService<CreateSetupService>.CreateCorrectService<TDto>(WhatItShouldBe.SyncSpecificDto, _db);
            return service.GetDto();
        }
    }

    public class CreateSetupService<TData, TDto> : ICreateSetupService<TData, TDto>
        where TData : class, new()
        where TDto : EfGenericDto<TData, TDto>, new()
    {
        private readonly IGenericServicesDbContext _db;

        public CreateSetupService(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This returns the dto with any data that is needs for the view setup in it
        /// </summary>
        /// <returns>A TDto which has had the SetupSecondaryData method called on it</returns>
        public TDto GetDto()
        {
            var dto = new TDto();
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                dto.SetupSecondaryData(_db, dto);

            return dto;
        }
    }
}
