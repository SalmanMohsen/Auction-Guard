using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.IServices
{
    /// <summary>
    /// A dedicated client for interacting with the PayPal API for both Merchant and Platform operations.
    /// </summary>
    public interface IPayPalClientService
    {
        /// <summary>
        /// Generates a direct PayPal authorization URL for a seller to grant permissions.
        /// </summary>
        /// <returns>The authorization URL the user should be redirected to.</returns>
        string GenerateOnboardingAuthorizationUrl();

        /// <summary>
        /// Exchanges the authorization code from PayPal for an access token and retrieves the seller's merchant ID.
        /// </summary>
        /// <param name="authorizationCode">The code returned from PayPal after user consent.</param>
        /// <returns>The seller's unique PayPal Merchant ID (Payer ID).</returns>
        Task<(string? merchantId, string? error)> GetMerchantIdFromAuthCode(string authorizationCode);

        
        /// <summary>
        /// Creates a PayPal order with an AUTHORIZE intent for a guarantee deposit.
        /// Now returns a detailed error message on failure.
        /// </summary>
        Task<(string? orderId, string? approvalUrl, string? error)> CreateDepositOrderAsync(decimal amount, string currency, Guid auctionId, Guid userId);

        /// <summary>
        /// Authorizes (places a hold on) a previously created and user-approved order.
        /// </summary>
        Task<(string? authorizationId, string? error)> AuthorizeDepositOrderAsync(string orderId);

        /// <summary>
        /// Voids a previously created authorization, releasing the hold on the funds.
        /// </summary>
        Task<bool> VoidDepositAuthorizationAsync(string authorizationId);
        
    }
}
