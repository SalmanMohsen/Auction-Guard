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
    /// Represents a bid placed by a user in an auction.
    /// </summary>
    public class Bid
    {
        #region Properties
        /// <summary>
        /// Gets or sets the primary key for the bid.
        /// </summary>
        public Guid BidId { get; set; }

        /// <summary>
        /// Gets or sets the amount of the bid.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the bid was placed.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        #endregion

        #region Foreign Keys & Navigation Properties
        /// <summary>
        /// Gets or sets the identifier of the auction this bid belongs to.
        /// </summary>
        public Guid AuctionId { get; set; } 

        /// <summary>
        /// Navigation property for the auction.
        /// </summary>
        public virtual Auction Auction { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who placed this bid.
        /// </summary>
        [Required]
        public Guid BidderId { get; set; } 
        #endregion

    }
}
