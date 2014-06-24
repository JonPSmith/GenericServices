using System.Threading.Tasks;
using GenericServices.Core;
using GenericServices.ServicesAsync;

namespace GenericServices
{

    public interface ICreateServiceAsync<in TData> where TData : class
    {
        Task<ISuccessOrErrors> CreateAsync(TData newItem);
    }

    public interface ICreateServiceAsync<TData, TDto>
        where TData : class, new()
        where TDto : EfGenericDtoAsync<TData, TDto>
    {
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