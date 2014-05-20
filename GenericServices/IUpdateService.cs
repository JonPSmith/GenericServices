using GenericServices.Concrete;

namespace GenericServices
{
    public interface IUpdateService<in TData> where TData : class
    {
        ISuccessOrErrors Update(TData itemToUpdate);
    }

    public interface IUpdateService<TData, TDto>
        where TData : class
        where TDto : EfGenericDto<TData, TDto>
    {
        ISuccessOrErrors Update(TDto dto);

        /// <summary>
        /// This is available to reset any secondary data in the dto. Call this if the ModelState was invalid and
        /// you need to display the view again with errors
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        TDto ResetDto(TDto dto);
    }
}