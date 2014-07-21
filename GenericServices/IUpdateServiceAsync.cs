using System.Threading.Tasks;
using GenericServices.Core;
using GenericServices.ServicesAsync;

namespace GenericServices
{
    public interface IUpdateServiceAsync
    {
        /// <summary>
        /// This updates the data in the database using the input data
        /// </summary>
        /// <typeparam name="T">The type of input data. 
        /// Type must be a type either an EF data class or one of the EfGenericDto's</typeparam>
        /// <param name="data">data to update the class. If Dto then copied over to data class</param>
        /// <returns></returns>
        Task<ISuccessOrErrors> UpdateAsync<T>(T data) where T : class;
    }

    public interface IUpdateServiceAsync<in TData> where TData : class
    {
        /// <summary>
        /// This updates the entity data class directly
        /// </summary>
        /// <param name="itemToUpdate"></param>
        /// <returns>status</returns>
        Task<ISuccessOrErrors> UpdateAsync(TData itemToUpdate);
    }

    public interface IUpdateServiceAsync<TData,TDto>
        where TData : class
        where TDto : EfGenericDtoAsync<TData, TDto>
    {
        /// <summary>
        /// This updates the entity data by copying over the relevant dto data.
        /// If it fails it resets the dto in case it is going to be shown again
        /// </summary>
        /// <param name="dto">If an error then its resets any secondary data so that you can reshow the dto</param>
        /// <returns>status</returns>
        Task<ISuccessOrErrors> UpdateAsync(TDto dto);

        /// <summary>
        /// This is available to reset any secondary data in the dto. Call this if the ModelState was invalid and
        /// you need to display the view again with errors
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<TDto> ResetDtoAsync(TDto dto);
    }
}