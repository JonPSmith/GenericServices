using GenericServices.Concrete;

namespace GenericServices
{
    public interface ICreateService<in TData> where TData : class
    {
        ISuccessOrErrors Create(TData newItem);
    }

    public interface ICreateService<TData, in TDto>
        where TData : class, new()
        where TDto : EfGenericDto<TData, TDto>
    {
        ISuccessOrErrors Create(TDto dto);
    }
}