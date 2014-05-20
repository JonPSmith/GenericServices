using GenericServices.Concrete;

namespace GenericServices
{
    public interface ICreateService<in TData> where TData : class
    {
        ISuccessOrErrors Create(TData newItem);
    }

    public interface ICreateService<TData, TDto>
        where TData : class, new()
        where TDto : EfGenericDto<TData, TDto>
    {
        ISuccessOrErrors Create(TDto dto);

        /// <summary>
        /// This is available to reset any secondary data in the dto. Call this if the ModelState was invalid and
        /// you need to display the view again with errors
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        TDto ResetDto(TDto dto);
    }
}