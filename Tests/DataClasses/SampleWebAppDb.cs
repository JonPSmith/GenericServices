using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using GenericServices;
using GenericServices.Core;
using GenericServices.Services;
using Tests.DataClasses.Concrete;
using Tests.DataClasses.Concrete.Helpers;

namespace Tests.DataClasses
{

    public class SampleWebAppDb : DbContext, IDbContextWithValidation
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTagGrade> PostTagGrades { get; set; }

        public ISuccessOrErrors SaveChangesWithValidation()
        {
            var result = new SuccessOrErrors();
            var numChanges = 0;
            try
            {
                numChanges = SaveChanges(); //then update it
            }
            catch (DbEntityValidationException ex)
            {
                return result.SetErrors(ex.EntityValidationErrors);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is System.Data.Entity.Core.UpdateException &&
                    ex.InnerException.InnerException is System.Data.SqlClient.SqlException
                    && ex.InnerException.InnerException.Data["HelpLink.EvtID"] is string &&
                    (string) ex.InnerException.InnerException.Data["HelpLink.EvtID"] == "547")
                    return result.AddSingleError("This operation failed because other data uses this entry.");
           
                throw; //else it isn't something we understand
            }

            return result.SetSuccessMessage("Successfully added or updated {0} items", numChanges);
        }

        public async Task<ISuccessOrErrors> SaveChangesWithValidationAsync()
        {
            var result = new SuccessOrErrors();
            var numChanges = 0;
            try
            {
                numChanges = await SaveChangesAsync(); //then update it
            }
            catch (DbEntityValidationException ex)
            {
                return result.SetErrors(ex.EntityValidationErrors);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is System.Data.Entity.Core.UpdateException &&
                    ex.InnerException.InnerException is System.Data.SqlClient.SqlException
                    && ex.InnerException.InnerException.Data["HelpLink.EvtID"] is string &&
                    (string)ex.InnerException.InnerException.Data["HelpLink.EvtID"] == "547")
                    return result.AddSingleError("This operation failed because other data uses this entry.");

                throw; //else it isn't something we understand
            }

            return result.SetSuccessMessage("Successfully added or updated {0} items", numChanges);
        }

        /// <summary>
        /// This has been overridden to handle:
        /// a) Updating of modified items (see p194 in DbContext book)
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {

            HandleChangeTracking();

            return base.SaveChanges();

        }

        /// <summary>
        /// This does validations that can only be done at the database level
        /// </summary>
        /// <param name="entityEntry"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry,
            IDictionary<object, object> items)
        {

            if (entityEntry.Entity is Tag && (entityEntry.State == EntityState.Added || entityEntry.State == EntityState.Modified))
            {
                var tagToCheck = ((Tag)entityEntry.Entity);

                //check for uniqueness of Service shortName (note: because we may alter a service we need to exclude check against itself)
                if (Tags.Any(x => x.TagId != tagToCheck.TagId && x.Slug == tagToCheck.Slug))
                    return new DbEntityValidationResult(entityEntry,
                                                        new List<DbValidationError>
                                                            {
                                                                new DbValidationError( "Slug",
                                                                    string.Format( "The Slug on tag '{0}' must be unique.", tagToCheck.Name))
                                                            });
            }

            return base.ValidateEntity(entityEntry, items);
        }

        /// <summary>
        /// This handles going through all the entities that have changed and seeing if they need any special handling.
        /// </summary>
        private void HandleChangeTracking()
        {
            //Debug.WriteLine("----------------------------------------------");
            //foreach (var entity in ChangeTracker.Entries()
            //.Where(
            //    e =>
            //    e.State == EntityState.Added || e.State == EntityState.Modified))
            //{
            //    Debug.WriteLine("Entry {0}, state {1}", entity.Entity, entity.State);
            //}       

            foreach (var entity in ChangeTracker.Entries()
                                                .Where(
                                                    e =>
                                                    e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                var trackUpdateClass = entity.Entity as TrackUpdate;
                if (trackUpdateClass == null) return;
                trackUpdateClass.UpdateTrackingInfo();
            }


        }
    }
}
