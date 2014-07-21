using System.Threading.Tasks;
using GenericServices.Core;

namespace GenericServices
{
    public interface ICreateSetupServiceAsync
    {
        /// <summary>
        /// This returns the dto with any data that is needs for the view setup in it
        /// </summary>
        /// <typeparam name="TDto">The type of the data to output. This must be EfGeneric Dto</typeparam>
        /// <returns>The dto with any secondary data filled in</returns>
        Task<TDto> GetDtoAsync<TDto>() where TDto : class;
    }

    public interface ICreateSetupServiceAsync<TData, TDto>
        where TData : class
        where TDto : EfGenericDtoAsync<TData, TDto>, new()
    {
        /// <summary>
        /// This returns the dto with any data that is needs for the view setup in it
        /// </summary>
        /// <returns>An async Task TDto which has had the SetupSecondaryData method called on it</returns>
        Task<TDto> GetDtoAsync();
    }
}