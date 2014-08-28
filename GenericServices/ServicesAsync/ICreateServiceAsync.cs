using System.Threading.Tasks;
using GenericServices.Core;

namespace GenericServices.ServicesAsync
{

    public interface ICreateServiceAsync<in TData> where TData : class
    {
        /// <summary>
        /// This adds a new entity class to the database with error checking
        /// </summary>
        /// <param name="newItem"></param>
        /// <returns>status</returns>
        Task<ISuccessOrErrors> CreateAsync(TData newItem);
    }

    public interface ICreateServiceAsync<TData, TDto>
        where TData : class, new()
        where TDto : EfGenericDtoAsync<TData, TDto>, new()
    {

        /// <summary>
        /// This uses a dto to create a data class which it writes to the database with error checking
        /// </summary>
        /// <param name="dto">If an error then its resets any secondary data so that you can reshow the dto</param>
        /// <returns>status</returns>
        Task<ISuccessOrErrors> CreateAsync(TDto dto);

        /// <summary>
        /// This is available to reset any secondary data in the dto. Call this if the ModelState was invalid and
        /// you need to display the view again with errors
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<TDto> ResetDtoAsync(TDto dto);
    }
}