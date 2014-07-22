using GenericServices.Core;

namespace GenericServices.Services
{   

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