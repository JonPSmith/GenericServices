using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericServices;
using GenericServices.Concrete;

namespace Tests.Helpers
{
    public class DummyIDbContextWithValidation : IDbContextWithValidation
    {

        public bool SaveChangesWithValidationCalled { get; private set; }

        public ISuccessOrErrors SaveChangesWithValidation()
        {
            SaveChangesWithValidationCalled = true;
            return SuccessOrErrors.Success("All ok.");
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            throw new NotImplementedException();
        }

        public DbSet Set(Type entityType)
        {
            throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DbEntityValidationResult> GetValidationErrors()
        {
            throw new NotImplementedException();
        }

        public DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public DbEntityEntry Entry(object entity)
        {
            throw new NotImplementedException();
        }
    }
}
