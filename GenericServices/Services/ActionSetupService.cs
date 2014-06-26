using GenericServices.Core;

namespace GenericServices.Services
{

    public class ActionSetupService<TData, TDto> : CreateSetupService<TData, TDto>, IActionSetupService<TData, TDto>
        where TData : class
        where TDto : EfGenericDto<TData, TDto>, new()
    {
        public ActionSetupService(IDbContextWithValidation db) : base(db)
        {
        }
    }
}
