using GenericServices.Core;

namespace GenericServices.ServicesAsync.Concrete
{

    public class ActionSetupServiceAsync<TData, TDto> : CreateSetupServiceAsync<TData, TDto>, IActionSetupServiceAsync<TData, TDto>
        where TData : class, new()
        where TDto : EfGenericDtoAsync<TData, TDto>, new()
    {
        public ActionSetupServiceAsync(IDbContextWithValidation db) : base(db)
        {
        }
    }
}
