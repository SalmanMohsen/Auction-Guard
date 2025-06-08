using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Domain.Entities
{

    /// <summary>
    /// Represents the current status of an auction.
    /// </summary>
    public enum AuctionStatus 
    { 
        Scheduled, 
        Active, 
        Ended, 
        Cancelled 
    }

    /// <summary>
    /// Represents an auction event for a property.
    /// </summary>
    public class Auction
    {
        #region Properties
        /// <summary>
        /// Gets or sets the primary key for the auction.
        /// </summary>
        public Guid AuctionId { get; set; }

        /// <summary>
        /// Gets or sets the date and time the auction is scheduled to start.
        /// </summary>
        [Required]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the date and time the auction is scheduled to end.
        /// </summary>
        [Required]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets the minimum amount by which a new bid must exceed the current one.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? MinBidIncrement { get; set; }

        /// <summary>
        /// Gets or sets the current status of the auction (e.g., 'Scheduled', 'Active', 'Ended').
        /// </summary>
        [Required]
        public AuctionStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the date and time this auction was created in the system.
        /// </summary>
        [Required]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets an optional deposit amount required to participate.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? GuaranteeDeposit { get; set; }
        #endregion

        #region Foreign Keys & Navigation Properties
        /// <summary>
        /// Gets or sets the foreign key for the property being auctioned.
        /// </summary>
        [Required]
        public Guid PropertyId { get; set; }
        /// <summary>
        /// Gets or sets the navigation property for the property being auctioned.
        /// </summary>
        public Property Property { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the user who created the auction.
        /// </summary>
        [Required]
        public Guid CreatorId { get; set; }
        
        /// <summary>
        /// Gets or sets the foreign key for the winning user. Null until the auction is won.
        /// </summary>
        public Guid? WinnerId { get; set; }
        #endregion

        #region Navigation Collections
        
        public ICollection<Bid> Bids { get; set; } = new HashSet<Bid>();
        
        public ICollection<Offer> Offers { get; set; } = new HashSet<Offer>();
        public ICollection<Invoice> Invoices { get; set; } = new HashSet<Invoice>();
        public ICollection<BidderAuthorization> BidderAuthorizations { get; set; } = new HashSet<BidderAuthorization>();
        public ICollection<AuctionParticipant> Participants { get; set; } = new HashSet<AuctionParticipant>();
        public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
        #endregion
    }
}
