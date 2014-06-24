using System.Threading.Tasks;
using GenericServices.Core;
using GenericServices.Services;

namespace GenericServices.ServicesAsync
{
    public class DeleteServiceAsync<TData> : IDeleteServiceAsync<TData> where TData : class, new()                                            
    {
        private readonly IDbContextWithValidation _db;

        public DeleteServiceAsync(IDbContextWithValidation db)
        {
            _db = db;
        }

        public async Task<ISuccessOrErrors> DeleteAsync(params object [] keys)
        {
            ISuccessOrErrors result = new SuccessOrErrors();

            var itemToDelete = await _db.Set<TData>().FindAsync(keys);
            if (itemToDelete == null)
                return result.AddSingleError("Could not find the {0} you asked to delete.", typeof(TData).Name);

            _db.Set<TData>().Remove(itemToDelete);
            result = await _db.SaveChangesWithValidationAsync();
            if (result.IsValid)
                result.SetSuccessMessage("Successfully deleted {0}.", typeof(TData).Name);

            return result;

        }

    }
}
