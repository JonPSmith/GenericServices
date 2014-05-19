namespace GenericServices.Concrete
{
    public class CreateService<TData> : ICreateService<TData> where TData : class
    {
        private readonly IDbContextWithValidation _db;

        public CreateService(IDbContextWithValidation db)
        {
            _db = db;
        }

        public ISuccessOrErrors Create(TData newItem)
        {
            _db.Set<TData>().Add(newItem);
            var result = _db.SaveChangesWithValidation();
            if (result.IsValid)
                result.SetSuccessMessage("Successfully created {0}.", typeof(TData).Name);

            return result;
        }

    }

    //---------------------------------------------------------------------------

    public class CreateService<TData, TDto> : ICreateService<TData, TDto>
        where TData : class, new()
        where TDto : EfGenericDto<TData, TDto>
    {
        private readonly IDbContextWithValidation _db;


        public CreateService(IDbContextWithValidation db)
        {
            _db = db;
        }

        public ISuccessOrErrors Create(TDto dto)
        {
            ISuccessOrErrors result = new SuccessOrErrors();
            if (!dto.SupportedFunctions.HasFlag(CrudFunctions.Create))
                return result.AddSingleError("Create of a new {0} is not supported in this mode.", dto.DataItemName);
            
            var tData = new TData();
            result = dto.CopyDtoToData(_db, dto, tData);    //update those properties we want to change
            if (!result.IsValid)
                return result;

            _db.Set<TData>().Add(tData);
            result = _db.SaveChangesWithValidation();
            if (result.IsValid)
                result.SetSuccessMessage("Successfully created {0}.", dto.DataItemName);

            return result;

        }

    }
}
