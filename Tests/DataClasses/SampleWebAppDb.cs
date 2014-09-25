using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using GenericServices;
using Tests.DataClasses.Concrete;
using Tests.DataClasses.Concrete.Helpers;

namespace Tests.DataClasses
{

    public class SampleWebAppDb : DbContext, IGenericServicesDbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTagGrade> PostTagGrades { get; set; }
        public DbSet<PostLink> PostLinks { get; set; }

        

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

        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            //We need to override the Cascade deletes so we can check out delete of entry that has non-nullable foreign key pointing to it
            modelBuilder.Entity<PostLink>()
                .HasRequired( x => x.PostPart)
                .WithMany()
                .HasForeignKey(x => x.PostId)
                .WillCascadeOnDelete(false);
        }

        //--------------------------------------------------
        //private helpers

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
