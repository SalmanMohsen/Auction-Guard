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
    /// Represents an invoice for a user.
    /// </summary>
    public class Invoice
    {
        #region Properties
        /// <summary>
        /// Gets or sets the primary key for the invoice.
        /// </summary>
        public Guid InvoiceId { get; set; }

        /// <summary>
        /// Gets or sets the human-readable, unique invoice number.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the date the invoice was created.
        /// </summary>
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the total amount due for this invoice.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal AmountDue { get; set; }

        /// <summary>
        /// Gets or sets a description of the invoice contents.
        /// </summary>
        [Required,MaxLength(700)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date the invoice was sent to the user.
        /// </summary>
        [Required]
        public DateTime SentOn { get; set; }

        /// <summary>
        /// Gets or sets the date the payment for this invoice is due.
        /// </summary>
        [Required]
        public DateTime DueDate { get; set; }
        #endregion

        #region Foreign Keys & Navigation Properties
        /// <summary>
        /// Gets or sets the foreign key for the user this invoice is issued to.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the user.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the auction that generated this invoice.
        /// </summary>
        [Required]
        public Guid AuctionId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the auction.
        /// </summary>
        public Auction Auction { get; set; }
        #endregion

        #region Navigation Collections
        /// <summary>
        /// Gets the collection of payment transactions applied to this invoice.
        /// </summary>
        public ICollection<PaymentTransaction> Transactions { get; private set; } = new HashSet<PaymentTransaction>();
        #endregion

    }
}
