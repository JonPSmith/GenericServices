using System.Threading.Tasks;

namespace GenericServices
{
    public interface IDeleteServiceAsync<TData> where TData : class, new()
    {
        Task<ISuccessOrErrors> DeleteAsync(params object [] keys);
    }
}