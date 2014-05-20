namespace GenericServices.Concrete
{
    public class CreateSetupService<TData, TDto> : ICreateSetupService<TData, TDto> where TData : class
        where TDto : EfGenericDto<TData, TDto>
    {
        private readonly IDbContextWithValidation _db;
        private readonly TDto _tDto;

        public CreateSetupService(IDbContextWithValidation db, TDto tDto)
        {
            _db = db;
            _tDto = tDto;
        }

        /// <summary>
        /// This returns the dto with any data that is needs for the view setup in it
        /// </summary>
        /// <returns>A TDto which has had the SetupSecondaryData method called on it</returns>
        public TDto GetDto()
        {
            _tDto.SetupSecondaryData(_db, _tDto);

            return _tDto;
        }
    }
}
