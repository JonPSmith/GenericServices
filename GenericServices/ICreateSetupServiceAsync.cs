using System.Threading.Tasks;

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
}