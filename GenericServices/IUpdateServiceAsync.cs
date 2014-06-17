using System.Threading.Tasks;
using GenericServices.ServicesAsync;

namespace GenericServices
{
    public interface IUpdateServiceAsync<in TData> where TData : class
    {
        Task<ISuccessOrErrors> UpdateAsync(TData itemToUpdate);
    }

    public interface IUpdateServiceAsync<TData,TDto>
        where TData : class
        where TDto : EfGenericDtoAsync<TData, TDto>
    {
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