using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.IServices
{
    /// <summary>
    /// Defines the contract for a payment gateway service that can authorize and release funds.
    /// </summary>
    public interface IPaymentGatewayService
    {
        /// <summary>
        /// Creates a payment order with the intent to authorize funds.
        /// </summary>
        Task<(string? orderId, string? errorMessage)> CreateAuthorizationOrder(decimal amount);

        /// <summary>
        /// Finalizes the authorization for a previously created and user-approved order.
        /// </summary>
        Task<(string? gatewayAuthorizationId, string? errorMessage)> AuthorizeOrder(string orderId);

        /// <summary>
        /// Releases a previously authorized payment hold.
        /// </summary>
        Task<(bool success, string? errorMessage)> ReleaseAuthorization(string gatewayAuthorizationId);
    }
}
