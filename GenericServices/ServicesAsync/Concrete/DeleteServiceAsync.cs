using System;
using System.Data.Entity;
using System.Threading.Tasks;
using GenericServices.Core;
using GenericServices.Core.Internal;

namespace GenericServices.ServicesAsync.Concrete
{

    public class DeleteServiceAsync : IDeleteServiceAsync
    {
        private readonly IGenericServicesDbContext _db;

        public DeleteServiceAsync(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This will delete an item from the database
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns></returns>
        public async Task<ISuccessOrErrors> DeleteAsync<TData>(params object[] keys) where TData : class, new()  
        {
            var keyProperties = _db.GetKeyProperties<TData>();
            if (keyProperties.Count != keys.Length)
                throw new ArgumentException("The number of keys in the data entry did not match the number of keys provided");

            var entityToDelete = await _db.Set<TData>().FindAsync(keys);
            if (entityToDelete == null)
                return
                    new SuccessOrErrors().AddSingleError(
                        "Could not delete entry as it was not in the database. Could it have been deleted by someone else?");

            _db.Set<TData>().Remove(entityToDelete);
            var result = await _db.SaveChangesWithCheckingAsync();
            if (result.IsValid)
                result.SetSuccessMessage("Successfully deleted {0}.", typeof(TData).Name);

            return result;
        }

    }
}
