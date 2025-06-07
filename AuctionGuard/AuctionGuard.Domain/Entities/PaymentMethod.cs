using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Domain.Entities
{
    /// <summary>
    /// Represents an available payment method.
    /// </summary>
    public class PaymentMethod
    {
        #region Properties
        /// <summary>
        /// Gets or sets the primary key for the payment method.
        /// </summary>
        public Guid MethodId { get; set; }

        /// <summary>
        /// Gets or sets the name of the payment method.
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets the URL for the payment method's icon.
        /// </summary>
        public string MethodIcon { get; set; }
        #endregion

        #region Navigation Collections
        /// <summary>
        /// Gets the collection of transactions made with this method.
        /// </summary>
        public ICollection<PaymentTransaction> Transactions { get; private set; } = new HashSet<PaymentTransaction>();
        #endregion
    }
}
