using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;

using swishes.Core.Entities.Wishes;
using swishes.Core.Entities.Profile;

namespace swishes.Infrastructure.DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("LocalConnection")
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
    }
}