using System;
using System.Linq;

namespace GenericServices.Concrete
{
    public class ListService<TData> : IListService<TData> where TData : class
    {
        private readonly IDbContextWithValidation _db;

        public ListService(IDbContextWithValidation db)
        {
            _db = db;
        }

        /// <summary>
        /// This returns an IQueryable list of all items of the given type
        /// </summary>
        /// <returns>note: items are not tracked</returns>
        public IQueryable<TData> GetList()
        {
            return _db.Set<TData>().AsNoTracking();
        }

    }

    //---------------------------------------------------------------------------

    public class ListService<TData, TDto> : IListService<TData, TDto>
        where TData : class
        where TDto : EfGenericDto<TData, TDto>, new()
    {
        private readonly IDbContextWithValidation _db;

        public ListService(IDbContextWithValidation db)
        {
            _db = db;
        }

        /// <summary>
        /// This returns an IQueryable list of all items of the given TData, but transformed into TDto data type
        /// </summary>
        /// <returns></returns>
        public IQueryable<TDto> GetList()
        {
            var tDto = new TDto();
            if (!tDto.SupportedFunctions.HasFlag(CrudFunctions.List))
                throw new InvalidOperationException("This DTO does not support listings.");

            return tDto.BuildListQueryUntracked(_db);
        }
    }

}
