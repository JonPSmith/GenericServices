namespace GenericServices.Concrete
{
    public class DeleteService<TData> : IDeleteService<TData> where TData : class, new()                                            
    {
        private readonly IDbContextWithValidation _db;

        public DeleteService(IDbContextWithValidation db)
        {
            _db = db;
        }

        public ISuccessOrErrors Delete(params object [] keys)
        {
            ISuccessOrErrors result = new SuccessOrErrors();

            var itemToDelete = _db.Set<TData>().Find(keys);
            if (itemToDelete == null)
                return result.AddSingleError("Could not find the {0} you asked to delete.", typeof(TData).Name);

            var tData = new TData();
            _db.Set<TData>().Remove(itemToDelete);
            result = _db.SaveChangesWithValidation();
            if (result.IsValid)
                result.SetSuccessMessage("Successfully deleted {0}.", typeof(TData).Name);

            return result;

        }

    }
}
