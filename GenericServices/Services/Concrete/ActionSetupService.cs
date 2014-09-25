using GenericServices.Core;

namespace GenericServices.Services.Concrete
{

    public class ActionSetupService<TData, TDto> : CreateSetupService<TData, TDto>, IActionSetupService<TData, TDto>
        where TData : class, new()
        where TDto : EfGenericDto<TData, TDto>, new()
    {
        public ActionSetupService(IGenericServicesDbContext db) : base(db)
        {
        }
    }
}
