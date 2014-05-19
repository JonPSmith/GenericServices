using GenericServices.Concrete;

namespace GenericServices
{
    public interface IUpdateService<in TData> where TData : class
    {
        ISuccessOrErrors Update(TData itemToUpdate);
    }

    public interface IUpdateService<TData, in TDto>
        where TData : class
        where TDto : EfGenericDto<TData, TDto>
    {
        ISuccessOrErrors Update(TDto dto);
    }
}