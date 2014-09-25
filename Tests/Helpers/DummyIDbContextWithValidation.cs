using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using GenericServices;
using GenericServices.Core;

namespace Tests.Helpers
{
    public class DummyIDbContextWithValidation : IGenericServicesDbContext
    {

        public bool SaveChangesCalled { get; private set; }

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
            SaveChangesCalled = true;
            return 1;
        }

        public async Task<int> SaveChangesAsync()
        {
            SaveChangesCalled = true;
            return 1;
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
