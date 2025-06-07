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
    /// Defines the type of notification.
    /// </summary>
    public enum NotificationType 
    { 
        NewBid, 
        AuctionEndingSoon, 
        AuctionWon, 
        Outbid, 
        NewPropertyListed, 
        AuctionStarted, 
        AuctionEnded, 
        SystemMessage 
    }

    /// <summary>
    /// Represents a notification sent to a user.
    /// </summary>
    public class Notification
    {
        #region Properties
        /// <summary>
        /// Gets or sets the unique identifier for the notification.
        /// </summary>
        public Guid NotificationId { get; set; }

        /// <summary>
        /// Gets or sets the notification message.
        /// </summary>
        [Required]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the type of the notification.
        /// </summary>
        public NotificationType Type { get; set; }

        /// <summary>
        /// Gets or sets when the notification was sent.
        /// </summary>
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        #endregion

        #region Foreign Keys & Navigation Properties
        /// <summary>
        /// Gets or sets the identifier of the user receiving this notification.
        /// </summary>
        [Required]
        public Guid UserId { get; set; } 

        /// <summary>
        /// Navigation property for the user.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the related auction, if applicable.
        /// </summary>
        public Guid? AuctionId { get; set; } 

        /// <summary>
        /// Navigation property for the auction.
        /// </summary>
        public virtual Auction Auction { get; set; }
        #endregion



    }
}
