using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;

using swishes.Core.Entities.Wishes;
using swishes.Core.Entities.Profile;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Data.Metadata.Edm;

namespace swishes.Infrastructure.DataAccess
{
    public class SqlServerContext : DbContext, IDataContext
    {
        static SqlServerContext()
        {
            Database.SetInitializer<SqlServerContext>(null);
        }

        public SqlServerContext()
            :base("Name=SqlServerConnection")
        {

        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Wish> Wishes { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<OAuthMembership> OAuthMemberships { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserInRole> UsersInRoles { get; set; }
        public DbSet<WishList> WishLists { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<OAuthMembership>().HasKey(a => new { a.Provider, a.ProviderUserId });
            modelBuilder.Entity<UserInRole>().HasKey(a => new { a.UserId, a.RoleId });
        }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }


        public bool SetEntryState<T>(T entity, EntityState state) where T : class
        {
            var entry = base.Entry<T>(entity);
            if (entry != null)
            {
                entry.State = state;
                return true;
            }
            return false;
        }

        public T GetAttached<T>(T entity) where T : class
        {
            var objectContext = ((IObjectContextAdapter)this).ObjectContext;
            string className = typeof(T).Name;
            var container = objectContext.MetadataWorkspace.GetEntityContainer(objectContext.DefaultContainerName, DataSpace.CSpace);
            string setName = container.BaseEntitySets.Where(meta => meta.ElementType.Name == className).Select(meta => meta.Name).First();
            EntityKey entityKey = objectContext.CreateEntityKey(setName, entity);

            ObjectStateEntry objectStateEntry;
            if (!objectContext.ObjectStateManager.TryGetObjectStateEntry(entityKey, out objectStateEntry))
            {
                return null;
            }

            if (objectStateEntry.State == EntityState.Detached) 
            {
                return null;
            }

            return (T)objectStateEntry.Entity;
        }

        private EntityKey GetEntityKey<T>(T entity) where T : class
        {
            var objectContext = ((IObjectContextAdapter)this).ObjectContext;
            ObjectStateEntry objectStateEntry;
            if (null != entity && objectContext.ObjectStateManager.TryGetObjectStateEntry(entity, out objectStateEntry))
            {
                return objectStateEntry.EntityKey;
            }
            return null;
        }

        public void Detach<T>(T entity) where T : class
        {
            var entry = this.Entry(entity);
            if (entry != null && entry.State != EntityState.Detached)
            {
                ((IObjectContextAdapter)this).ObjectContext.Detach(entity);
            }
        }
    }
}