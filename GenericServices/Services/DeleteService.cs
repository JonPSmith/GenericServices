using System;
using System.Data.Entity;
using GenericServices.Core;
using GenericServices.Core.Internal;

namespace GenericServices.Services
{
    public class DeleteService<TData> : IDeleteService<TData> where TData : class, new()                                            
    {
        private readonly IDbContextWithValidation _db;

        public DeleteService(IDbContextWithValidation db)
        {
            _db = db;
        }

        /// <summary>
        /// This will delete an item from the database
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns></returns>
        public ISuccessOrErrors Delete(params object [] keys)
        {

            var keyProperties = _db.GetKeyProperties<TData>();
            if (keyProperties.Count != keys.Length)
                throw new ArgumentException("The number of keys in the data entry did not match the number of keys provided");

            ISuccessOrErrors result = new SuccessOrErrors();

            var entityToDelete = new TData();
            int paramCount = 0;
            foreach (var keyProperty in keyProperties)
                keyProperty.SetValue(entityToDelete, keys[paramCount++]);

            _db.Entry(entityToDelete).State = EntityState.Deleted;
            result = _db.SaveChangesWithValidation();
            if (result.IsValid)
                result.SetSuccessMessage("Successfully deleted {0}.", typeof(TData).Name);

            return result;

        }

    }
}
