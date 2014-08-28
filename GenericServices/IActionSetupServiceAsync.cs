using System.Threading.Tasks;
using GenericServices.Core;
using GenericServices.ServicesAsync;

namespace GenericServices
{
    public interface IActionSetupServiceAsync<TData, TDto>
        where TData : class, new()
        where TDto : EfGenericDtoAsync<TData, TDto>, new()
    {
        /// <summary>
        /// This returns the dto with any data that is needs for the view setup in it
        /// </summary>
        /// <returns>An async Task TDto which has had the SetupSecondaryData method called on it</returns>
        Task<TDto> GetDtoAsync();
    }
}