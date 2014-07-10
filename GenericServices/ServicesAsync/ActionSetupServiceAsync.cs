using System.Threading.Tasks;
using GenericServices.Core;

namespace GenericServices.ServicesAsync
{

    public class ActionSetupServiceAsync<TData, TDto> : CreateSetupServiceAsync<TData, TDto>, IActionSetupServiceAsync<TData, TDto>
        where TData : class
        where TDto : EfGenericDtoAsync<TData, TDto>, new()
    {
        public ActionSetupServiceAsync(IDbContextWithValidation db) : base(db)
        {
        }
    }
}
