using System.Linq;
using GenericServices.Core;
using GenericServices.Services;

namespace GenericServices
{

    public interface IListService
    {
        /// <summary>
        /// This works out what sort of service is needed from the type provided
        /// and returns an IQueryable list of all items of the given type
        /// </summary>
        /// <typeparam name="T">The type of the data to output. 
        /// Type must be a type either an EF data class or a class inherited from the EfGenericDto or EfGenericDtoAsync</typeparam>
        /// <returns>note: the list items are not tracked</returns>
        IQueryable<T> GetList<T>() where T : class, new();
    }

}