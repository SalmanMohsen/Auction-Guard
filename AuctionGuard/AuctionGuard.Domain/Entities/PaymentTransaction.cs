using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AuctionGuard.Domain.Entities
{

    /// <summary>
    /// Represents the status of a payment transaction.
    /// </summary>
    public enum TStatus 
    { 
        Pending, 
        Success, 
        Failed 
    }
    public enum TType
    {
        Payment,
        Refund
    }

    /// <summary>
    /// Represents a single, actual movement of money to pay for an invoice.
    /// </summary>
    public class PaymentTransaction
    {
        #region Properties
        /// <summary>
        /// Gets or sets the primary key for the transaction.
        /// </summary>
        public Guid TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the type of the transaction (e.g., 'Payment', 'Refund').
        /// </summary>
        public TType TransactionType { get; set; }

        /// <summary>
        /// Gets or sets the status of the transaction (Pending, Success, Failed)
        /// </summary>
        public TStatus TransactionStatus { get; set; }
        /// <summary>
        /// Gets or sets the amount of the transaction in the system's currency (e.g., EUR).
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the date of the transaction.
        /// </summary>
        public DateTime TransactionDate { get; set; }


        /// <summary>
        /// Gets or sets the unique transaction ID from the external payment gateway (e.g., Stripe, Adyen).
        /// The gateway provides a unique ID for every transaction it processes
        /// It acts as a unique receipt number or reference ID provided by the gateway
        /// </summary>
        public string GatewayTransactionId { get; set; }

        /// <summary>
        /// Gets or sets the original amount if the payment was made in cryptocurrency.
        /// </summary>
        public decimal? CryptoAmount { get; set; }

        /// <summary>
        /// Gets or sets the symbol of the cryptocurrency used (e.g., "BTC", "ETH").
        /// </summary>
        public string? CryptoCurrency { get; set; }
        #endregion

        #region Foreign Keys & Navigation Properties
        /// <summary>
        /// Gets or sets the foreign key for the invoice this transaction is for.
        /// </summary>
        public Guid InvoiceId { get; set; }
        /// <summary>
        /// Gets or sets the navigation property for the invoice.
        /// </summary>
        public Invoice Invoice { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the payment method used.
        /// </summary>
        public Guid MethodId { get; set; }
        /// <summary>
        /// Gets or sets the navigation property for the payment method.
        /// </summary>
        public PaymentMethod PaymentMethod { get; set; }
        #endregion
    }
}
