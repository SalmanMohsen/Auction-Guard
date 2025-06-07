using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Domain.Entities
{
    /// <summary>
    /// Represents a specific, granular permission that can be assigned to roles.
    /// </summary>
    public class Permission
    {
        #region Properties
        /// <summary>
        /// Gets or sets the primary key for the permission.
        /// </summary>
        public Guid PermissionId { get; set; }

        /// <summary>
        /// Gets or sets the unique name of the permission (e.g., "CanDeleteAuction").
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string PermissionName { get; set; }
        #endregion

        #region Navigation Collections
        /// <summary>
        /// Gets the collection of roles that have this permission.
        /// </summary>
        public ICollection<Role> Roles { get; set; } = new HashSet<Role>();
        #endregion
    }
}
