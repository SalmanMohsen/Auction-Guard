using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Domain.Entities
{
    public enum BiddingAuthStatus
    {
        active,
        released,
        captured
    }
    public class BidderAuthorization
    {
        #region Properties
        /// <summary>
        /// Gets or sets the primary key for the authorization record.
        /// </summary>
        public Guid BidderAuthId { get; set; }

        /// <summary>
        /// Gets or sets the lifecycle status of the hold (e.g., 'Active', 'Released', 'Captured').
        /// </summary>
        public BiddingAuthStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the unique authorization ID from the external payment gateway.
        /// </summary>
        public string GatewayAuthId { get; set; }

        /// <summary>
        /// Gets or sets the amount held on the user's card.
        /// </summary>
        public decimal AmountHeld { get; set; }

        /// <summary>
        /// Gets or sets when the hold was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when the hold will expire according to the gateway.
        /// </summary>
        public DateTime ExpiresAt { get; set; }
        #endregion

        #region Foreign Keys & Navigation Properties
        /// <summary>
        /// Gets or sets the foreign key for the user whose card was authorized.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the user.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the auction this authorization is for.
        /// </summary>
        [Required]
        public Guid AuctionId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the auction.
        /// </summary>
        public Auction Auction { get; set; }
        #endregion


    }
}
