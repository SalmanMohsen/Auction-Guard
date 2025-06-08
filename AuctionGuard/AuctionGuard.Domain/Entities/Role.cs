using Microsoft.AspNetCore.Identity;
using System;

namespace AuctionGuard.Domain.Entities
{
    /// <summary>
    /// Represents a role in the system, inheriting from IdentityRole to integrate with ASP.NET Core Identity.
    /// </summary>
    public class Role : IdentityRole<Guid>
    {
        #region Navigation Collections
        /// <summary>
        /// Gets the collection of permissions associated with this role.
        /// </summary>
        public ICollection<Permission> Permissions { get; set; } = new HashSet<Permission>();
        #endregion
    }
}
