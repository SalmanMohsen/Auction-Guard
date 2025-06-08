using AuctionGuard.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Infrastructure.Contexts
{
    /// <summary>
    /// This DbContext manages the security and identity concerns of the application.
    /// It is responsible for Users, Roles, and Permissions, and will be mapped
    /// to a separate, secure database.
    /// </summary>
    public class AuctionGuardIdentityDbContext : IdentityDbContext<User, Role, Guid>
    {
        #region DbSets
        /// <summary>
        /// Gets or sets the DbSet for managing Permissions.
        /// DbSets for Users and Roles are inherited from the base IdentityDbContext.
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }
        #endregion

        public AuctionGuardIdentityDbContext(DbContextOptions<AuctionGuardIdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // This is CRITICAL. It configures all the ASP.NET Core Identity tables first.
            base.OnModelCreating(builder);

            #region Relationship Configurations
            /// <summary>
            /// Configures the many-to-many relationship between Role and Permission
            /// using a dedicated join table named "RolePermissions".
            /// </summary>
            builder.Entity<Role>()
                .HasMany(r => r.Permissions)
                .WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    j => j
                        .HasOne<Permission>()
                        .WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Role>()
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.ToTable("RolePermissions", "identity");
                        j.HasKey("RoleId", "PermissionId");
                    }
                );
            #endregion
        }
    }
}
