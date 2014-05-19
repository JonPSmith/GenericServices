namespace GenericServices
{
    public interface IDeleteService<TData> where TData : class
    {
        ISuccessOrErrors Delete(params object [] keys);
    }
}