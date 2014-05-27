namespace GenericServices.Services
{
    public class CreateSetupService<TData, TDto> : ICreateSetupService<TData, TDto> where TData : class
        where TDto : EfGenericDto<TData, TDto>, new()
    {
        private readonly IDbContextWithValidation _db;

        public CreateSetupService(IDbContextWithValidation db)
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
