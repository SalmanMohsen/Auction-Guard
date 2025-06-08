using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Domain.Entities
{
    /// <summary>
    /// Represents a user who has been blacklisted.
    /// </summary>
    public class BlacklistEntry
    {
        #region Composite Primary Key
        /// <summary>
        /// Gets or sets the identifier of the blacklisted user.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets when the blacklist period started.
        /// </summary>
        public DateTime BlacklistStartDate { get; set; } = DateTime.UtcNow;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the reason for blacklisting.
        /// </summary>
        [Required]
        [StringLength(1000)]
        public string ReasonMessage { get; set; }

        /// <summary>
        /// Gets or sets when the blacklist period ends (null for permanent).
        /// </summary>
        public DateTime? BlacklistEndDate { get; set; }
        #endregion

    }
}
