using GenericServices.Services;

namespace GenericServices.ServicesAsync
{
    public class CreateSetupServiceAsync<TData, TDto> : CreateSetupService<TData, TDto>, ICreateSetupServiceAsync<TData, TDto> 
        where TData : class
        where TDto : EfGenericDtoAsync<TData, TDto>, new()
    {
        public CreateSetupServiceAsync(IDbContextWithValidation db) : base(db) {}
    }
}
