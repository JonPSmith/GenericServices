using System.Threading.Tasks;
using GenericServices.Core;

namespace GenericServices.ServicesAsync
{
    public class CreateSetupServiceAsync<TData, TDto> : ICreateSetupServiceAsync<TData, TDto> 
        where TData : class
        where TDto : EfGenericDtoAsync<TData, TDto>, new()
    {
                private readonly IDbContextWithValidation _db;

        public CreateSetupServiceAsync(IDbContextWithValidation db)
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
