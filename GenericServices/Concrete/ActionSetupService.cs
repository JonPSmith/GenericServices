using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericServices.Concrete
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
