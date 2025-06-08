using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Domain.Entities
{
    public class AuctionParticipant 
    {
        #region Composite Primary Key
        /// <summary>
        /// Gets or sets the foreign key for the auction. Part of the composite key.
        /// </summary>
        public Guid AuctionId { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the user. Part of the composite key.
        /// </summary>
        public Guid ParticipantId { get; set; }
        #endregion

        #region Payload Data
        /// <summary>
        /// Gets or sets the time the user registered for the auction.
        /// </summary>
        public DateTime RegistrationTime { get; set; }
        #endregion

        #region Navigation Properties
        /// <summary>
        /// Gets or sets the navigation property to the Auction.
        /// </summary>
        public Auction Auction { get; set; }
        #endregion
    }
}
