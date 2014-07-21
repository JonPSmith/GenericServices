using GenericServices.Core;

namespace GenericServices
{
    public interface ICreateSetupService
    {
        /// <summary>
        /// This returns the dto with any data that is needs for the view setup in it
        /// </summary>
        /// <typeparam name="TDto">The type of the data to output. This must be EfGeneric Dto</typeparam>
        /// <returns>The dto with any secondary data filled in</returns>
        TDto GetDto<TDto>() where TDto : class;
    }

    public interface ICreateSetupService<TData, out TDto> 
        where TData : class
        where TDto : EfGenericDto<TData, TDto>, new()
    {
        /// <summary>
        /// This returns the dto with any data that is needs for the view setup in it
        /// </summary>
        /// <returns>A TDto which, if required, will have SetupSecondaryData method called on it</returns>
        TDto GetDto();
    }
}