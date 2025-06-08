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
    /// Represents a user's favorited property (join table).
    /// Composite primary key (UserId, PropertyId) configured in DbContext.
    /// </summary>
    public class FavoriteProperty
    {
        #region Composite Primary Key
        /// <summary>
        /// Gets or sets the identifier of the user. Part of composite PK.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the property. Part of composite PK.
        /// </summary>
        public Guid PropertyId { get; set; }
        #endregion

        #region Payload Data
        /// <summary>
        /// Gets or sets when the property was favorited.
        /// </summary>
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        #endregion

        #region Navigation Properties

        /// <summary>
        /// Navigation property for the property.
        /// </summary>
        public virtual Property Property { get; set; }
        #endregion
    }
}
