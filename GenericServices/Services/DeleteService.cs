using System;
using System.Data.Entity;
using GenericServices.Core;
using GenericServices.Core.Internal;

namespace GenericServices.Services
{
    public interface IDeleteService
    {
        /// <summary>
        /// This will delete an item from the database
        /// </summary>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns></returns>
        ISuccessOrErrors Delete<TData>(params object[] keys) where TData : class, new();
    }

    public class DeleteService : IDeleteService
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
        public ISuccessOrErrors Delete<TData>(params object[] keys) where TData : class, new() 
        {

            var keyProperties = _db.GetKeyProperties<TData>();
            if (keyProperties.Count != keys.Length)
                throw new ArgumentException("The number of keys in the data entry did not match the number of keys provided");

            var entityToDelete = new TData();
            int paramCount = 0;
            foreach (var keyProperty in keyProperties)
                keyProperty.SetValue(entityToDelete, keys[paramCount++]);

            _db.Entry(entityToDelete).State = EntityState.Deleted;
            ISuccessOrErrors result = _db.SaveChangesWithValidation();
            if (result.IsValid)
                result.SetSuccessMessage("Successfully deleted {0}.", typeof(TData).Name);

            return result;

        }

    }
}
