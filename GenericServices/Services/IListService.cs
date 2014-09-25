using System.Linq;
using GenericServices.Core;

namespace GenericServices.Services
{

    public interface IListService<out TData> where TData : class
    {
        /// <summary>
        /// This returns an IQueryable list of all items of the given type
        /// </summary>
        /// <returns>note: the list items are not tracked</returns>
        IQueryable<TData> GetAll();
    }

    public interface IListService<TData, out TDto>
        where TData : class
        where TDto : EfGenericDtoBase<TData, TDto>
    {
        /// <summary>
        /// This returns an IQueryable list of all items of the given type
        /// </summary>
        /// <returns>note: the list items are not tracked</returns>
        IQueryable<TDto> GetAll();
    }
}