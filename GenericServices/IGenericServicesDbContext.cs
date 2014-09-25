using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Threading.Tasks;

namespace GenericServices
{
    public interface IGenericServicesDbContext
    {

        int SaveChanges();
        Task<int> SaveChangesAsync();

        IEnumerable<DbEntityValidationResult> GetValidationErrors();

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbSet Set(Type entityType);

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        DbEntityEntry Entry(object entity);

    }
}
