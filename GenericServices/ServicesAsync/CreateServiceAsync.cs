using System.Threading.Tasks;
using GenericServices.Services;

namespace GenericServices.ServicesAsync
{
    public class CreateServiceAsync<TData> : ICreateServiceAsync<TData> where TData : class
    {
        private readonly IDbContextWithValidation _db;

        public CreateServiceAsync(IDbContextWithValidation db)
        {
            _db = db;
        }

        public async Task<ISuccessOrErrors> CreateAsync(TData newItem)
        {
            _db.Set<TData>().Add(newItem);
            var result = await _db.SaveChangesWithValidationAsync();
            if (result.IsValid)
                result.SetSuccessMessage("Successfully created {0}.", typeof(TData).Name);

            return result;
        }

    }

    //---------------------------------------------------------------------------

    public class CreateServiceAsync<TData, TDto> : ICreateServiceAsync<TData, TDto> 
        where TData : class, new()
        where TDto : EfGenericDtoAsync<TData, TDto>
    {
        private readonly IDbContextWithValidation _db;


        public CreateServiceAsync(IDbContextWithValidation db)
        {
            _db = db;
        }

        public async Task<ISuccessOrErrors> CreateAsync(TDto dto)
        {
            ISuccessOrErrors result = new SuccessOrErrors();
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.Create))
                return result.AddSingleError("Create of a new {0} is not supported in this mode.", dto.DataItemName);
            
            var tData = new TData();
            result = dto.CopyDtoToData(_db, dto, tData);    //update those properties we want to change
            if (result.IsValid)
            {
                _db.Set<TData>().Add(tData);
                result = await _db.SaveChangesWithValidationAsync();
                if (result.IsValid)
                    return result.SetSuccessMessage("Successfully created {0}.", dto.DataItemName);
            }

            //otherwise there are errors
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                //we reset any secondary data as we expect the view to be reshown with the errors
                dto.SetupSecondaryData(_db, dto);
            return result;

        }

        /// <summary>
        /// This is available to reset any secondary data in the dto. Call this if the ModelState was invalid and
        /// you need to display the view again with errors
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public TDto ResetDto(TDto dto)
        {
            if (!dto.SupportedFunctions.HasFlag(ServiceFunctions.DoesNotNeedSetup))
                //we reset any secondary data as we expect the view to be reshown with the errors
                dto.SetupSecondaryData(_db, dto);

            return dto;
        }

    }
}
