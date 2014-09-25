using GenericServices.Core;

namespace GenericServices.Services.Concrete
{

    public class DeleteService : IDeleteService
    {
        private readonly IGenericServicesDbContext _db;

        public DeleteService(IGenericServicesDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This will delete an item from the database
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns></returns>
        public ISuccessOrErrors Delete<TData>(params object[] keys) where TData : class, new() 
        {

            var entityToDelete = _db.Set<TData>().Find(keys);
            if (entityToDelete == null)
                return
                    new SuccessOrErrors().AddSingleError(
                        "Could not delete entry as it was not in the database. Could it have been deleted by someone else?");

            _db.Set<TData>().Remove(entityToDelete);
            var result = _db.SaveChangesWithChecking();
            if (result.IsValid)
                result.SetSuccessMessage("Successfully deleted {0}.", typeof(TData).Name);

            return result;

        }

    }
}
