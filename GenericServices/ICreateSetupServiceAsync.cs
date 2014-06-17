using GenericServices.ServicesAsync;

namespace GenericServices
{
    public interface ICreateSetupServiceAsync<TData, out TDto>
        where TData : class
        where TDto : EfGenericDtoAsync<TData, TDto>, new()
    {
        /// <summary>
        /// This returns the dto with any data that is needs for the view setup in it
        /// </summary>
        /// <returns>A TDto which has had the SetupSecondaryData method called on it</returns>
        TDto GetDto();
    }
}