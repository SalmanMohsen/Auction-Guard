
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuctionGuard.Domain.Entities
{
    /// <summary>
    /// Represents a user in the system. Inherits from IdentityUser to leverage
    /// ASP.NET Core's robust authentication and user management system.
    /// </summary>
    public class User : IdentityUser<Guid>
    {
        #region Custom Properties
        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        [MaxLength(50)]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's middle name.
        /// </summary>
        [MaxLength(50)]
        public string? MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        [MaxLength(50)]
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the URL for the user's uploaded identification document image.
        /// </summary>
        [Required]
        public string? IdentificationImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the date and time the user registered.
        /// </summary>
        [Required]
        public DateTime RegisterDate { get; set; }
        #endregion

        #region Navigation Collections
        /// <summary>
        /// Gets the collection of auctions created by this user.
        /// </summary>
        public ICollection<Auction> AuctionsCreated { get;  set; } = new HashSet<Auction>();

        /// <summary>
        /// Gets the collection of auctions won by this user.
        /// </summary>
        public ICollection<Auction> AuctionsWon { get;  set; } = new HashSet<Auction>();

        /// <summary>
        /// Gets the collection of properties owned by this user.
        /// </summary>
        public ICollection<Property> Properties { get; set; } = new HashSet<Property>();

        /// <summary>
        /// Gets the collection of bids placed by this user.
        /// </summary>
        public ICollection<Bid> Bids { get; set; } = new HashSet<Bid>();

        /// <summary>
        /// Gets the collection of invoices issued to this user.
        /// </summary>
        public ICollection<Invoice> Invoices { get; set; } = new HashSet<Invoice>();

        /// <summary>
        /// Gets the collection of payment authorizations for this user.
        /// </summary>
        public ICollection<BidderAuthorization> Authorizations { get; set; } = new HashSet<BidderAuthorization>();

        /// <summary>
        /// Gets the collection of blacklist entries (punishment history) for this user.
        /// </summary>
        public ICollection<BlacklistEntry> BlacklistHistory { get; set; } = new HashSet<BlacklistEntry>();

        /// <summary>
        /// Gets the collection of join entities representing the properties this user has favorited.
        /// </summary>
        public ICollection<FavoriteProperty> FavoriteProperties { get; set; } = new HashSet<FavoriteProperty>();

        /// <summary>
        /// Gets the collection of reviews written by this user.
        /// </summary>
        public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();

        /// <summary>
        /// Gets the collection of notifications for this user.
        /// </summary>
        public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();

        /// <summary>
        /// Gets the collection of join entities representing the auctions this user has participated in.
        /// </summary>
        public ICollection<AuctionParticipant> AuctionParticipations { get; set; } = new HashSet<AuctionParticipant>();

        /// <summary>
        /// Gets the collection of role assignments for this user, managed by ASP.NET Core Identity.
        /// </summary>
        public ICollection<IdentityUserRole<Guid>> Roles { get; set; } = new HashSet<IdentityUserRole<Guid>>();
        #endregion
    }
}
