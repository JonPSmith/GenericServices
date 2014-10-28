#region licence
// The MIT License (MIT)
// 
// Filename: DeleteServiceAsync.cs
// Date Created: 2014/07/22
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
using System;
using System.Threading.Tasks;
using GenericLibsBase;
using GenericLibsBase.Core;
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
        public async Task<ISuccessOrErrors> DeleteAsync<TEntity>(params object[] keys) where TEntity : class
        {
            var keyProperties = _db.GetKeyProperties<TEntity>();
            if (keyProperties.Count != keys.Length)
                throw new ArgumentException("The number of keys in the data entry did not match the number of keys provided");

            var entityToDelete = await _db.Set<TEntity>().FindAsync(keys);
            if (entityToDelete == null)
                return
                    new SuccessOrErrors().AddSingleError(
                        "Could not delete entry as it was not in the database. Could it have been deleted by someone else?");

            _db.Set<TEntity>().Remove(entityToDelete);
            var result = await _db.SaveChangesWithCheckingAsync();
            if (result.IsValid)
                result.SetSuccessMessage("Successfully deleted {0}.", typeof(TEntity).Name);

            return result;
        }

        /// <summary>
        /// This allows a developer to delete an entity plus any of its relationships.
        /// The first part of the method finds the given entity using the provided keys.
        /// It then calls the deleteRelationships method which should remove the extra relationships
        /// </summary>
        /// <param name="removeRelationshipsAsync">method which is handed the DbContext and the found entity.
        /// It should then remove any relationships on this entity that it wants to.
        /// It returns a status, if IsValid then calls SaveChangesWithChecking</param>
        /// <param name="keys">The keys must be given in the same order as entity framework has them</param>
        /// <returns></returns>
        public async Task<ISuccessOrErrors> DeleteWithRelationshipsAsync<TEntity>(Func<IGenericServicesDbContext, TEntity, Task<ISuccessOrErrors>> removeRelationshipsAsync,
            params object[] keys) where TEntity : class
        {

            var entityToDelete = await _db.Set<TEntity>().FindAsync(keys);
            if (entityToDelete == null)
                return
                    new SuccessOrErrors().AddSingleError(
                        "Could not delete entry as it was not in the database. Could it have been deleted by someone else?");

            var result = await removeRelationshipsAsync(_db, entityToDelete);
            if (!result.IsValid) return result;

            _db.Set<TEntity>().Remove(entityToDelete);
            result = await _db.SaveChangesWithCheckingAsync();
            if (result.IsValid)
                result.SetSuccessMessage("Successfully deleted {0} and given relationships.", typeof(TEntity).Name);

            return result;

        }
    
    }
}
