using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Domain.Entities
{
    public class Offer
    {
        #region Properties
        /// <summary>
        /// Gets or sets the primary key for the offer.
        /// </summary>
        public Guid OfferId { get; set; } 

        /// <summary>
        /// Gets or sets the description or terms of the offer.
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the price at which this offer can be triggered.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TriggerPrice { get; set; }
        #endregion

        #region Foreign Keys & Navigation Properties

        /// <summary>
        /// Gets or sets the foreign key for the auction this offer is for.
        /// </summary>
        [Required]
        public Guid AuctionId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the parent auction.
        /// </summary>
        public virtual Auction Auction { get; set; }
        #endregion
    }
}
