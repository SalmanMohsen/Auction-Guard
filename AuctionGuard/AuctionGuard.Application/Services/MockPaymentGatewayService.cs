using AuctionGuard.Application.IServices;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.Services
{
    public class MockPaymentGatewayService : IPaymentGatewayService
    {
        // Use ConcurrentDictionary for thread safety.
        // This static dictionary will persist across requests to simulate a real service's state.

        // 1. Stores orders that are created but not yet approved (authorized).
        private static readonly ConcurrentDictionary<string, decimal> _pendingOrders = new();

        // 2. Stores approved/authorized holds and their status (held or released).
        // Key: gatewayAuthorizationId, Value: (Amount, IsReleased)
        public static readonly ConcurrentDictionary<string, (decimal amount, bool isReleased)> AuthorizedHolds = new();

        /// <summary>
        /// Creates a fake order ID and stores the amount in a pending state.
        /// </summary>
        public Task<(string? orderId, string? errorMessage)> CreateAuthorizationOrder(decimal amount)
        {
            if (amount <= 0)
            {
                return Task.FromResult<(string?, string?)>((null, "Amount must be greater than zero."));
            }

            var orderId = $"mock_order_{Guid.NewGuid()}";
            _pendingOrders[orderId] = amount;

            // In a real scenario, this orderId would be sent to the frontend.
            return Task.FromResult<(string?, string?)>((orderId, null));
        }

        /// <summary>
        /// "Authorizes" the payment, moving it from pending to an active hold.
        /// This simulates the user approving the payment on the frontend.
        /// </summary>
        public Task<(string? gatewayAuthorizationId, string? errorMessage)> AuthorizeOrder(string orderId)
        {
            if (string.IsNullOrEmpty(orderId) || !_pendingOrders.TryRemove(orderId, out var amount))
            {
                return Task.FromResult<(string?, string?)>((null, "Invalid or already processed Order ID."));
            }

            var gatewayAuthorizationId = $"mock_auth_{Guid.NewGuid()}";
            AuthorizedHolds[gatewayAuthorizationId] = (amount, isReleased: false);

            return Task.FromResult<(string?, string?)>((gatewayAuthorizationId, null));
        }

        /// <summary>
        /// "Releases" a previously held amount by marking it as released.
        /// </summary>
        public Task<(bool success, string? errorMessage)> ReleaseAuthorization(string gatewayAuthorizationId)
        {
            if (string.IsNullOrEmpty(gatewayAuthorizationId) || !AuthorizedHolds.ContainsKey(gatewayAuthorizationId))
            {
                return Task.FromResult((false, "Authorization ID not found."));
            }

            var hold = AuthorizedHolds[gatewayAuthorizationId];
            if (hold.isReleased)
            {
                return Task.FromResult((false, "Funds have already been released for this authorization."));
            }

            // Update the hold status to released
            AuthorizedHolds[gatewayAuthorizationId] = (hold.amount, isReleased: true);

            return Task.FromResult((true, "Funds successfully released."));
        }
    }
}
