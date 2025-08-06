using AuctionGuard.Application.IServices;
using AuctionGuard.Domain.Entities;
using Microsoft.Extensions.Options;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionGuard.Infrastructure.Services
{
    public class PayPalService : IPaymentGatewayService
    {
        private readonly PayPalHttpClient _payPalClient;

        public PayPalService(IOptions<PayPalSettings> settings)
        {
            var settingsValue = settings.Value;
            var environment = settingsValue.Mode == "live"
                ? new LiveEnvironment(settingsValue.ClientId, settingsValue.ClientSecret)
                : (PayPalEnvironment)new SandboxEnvironment(settingsValue.ClientId, settingsValue.ClientSecret);

            _payPalClient = new PayPalHttpClient(environment);
        }

        public async Task<(string? orderId, string? errorMessage)> CreateAuthorizationOrder(decimal amount)
        {
            try
            {
                var orderRequest = new OrdersCreateRequest();
                orderRequest.Prefer("return=representation");
                orderRequest.RequestBody(BuildAuthorizationRequestBody(amount));

                var response = await _payPalClient.Execute(orderRequest);
                var result = response.Result<Order>();
                return (result.Id, null);
            }
            catch (Exception ex) { return (null, $"PayPal Error: {ex.Message}"); }
        }

        public async Task<(string? gatewayAuthorizationId, string? errorMessage)> AuthorizeOrder(string orderId)
        {
            try
            {
                var request = new OrdersAuthorizeRequest(orderId);
                request.RequestBody(new AuthorizeRequest());
                var response = await _payPalClient.Execute(request);
                var result = response.Result<Order>();

                var authorizationId = result.PurchaseUnits?.FirstOrDefault()
                                        ?.Payments?.Authorizations?.FirstOrDefault()?.Id;
                if (string.IsNullOrEmpty(authorizationId))
                    return (null, "Failed to retrieve Authorization ID from PayPal after user approval.");

                return (authorizationId, null);
            }
            catch (Exception ex) { return (null, $"PayPal Error: {ex.Message}"); }
        }

        public async Task<(bool success, string? errorMessage)> ReleaseAuthorization(string gatewayAuthorizationId)
        {
            try
            {
                var request = new AuthorizationsVoidRequest(gatewayAuthorizationId);
                var response = await _payPalClient.Execute(request);
                return response.StatusCode == System.Net.HttpStatusCode.NoContent
                    ? (true, null)
                    : (false, "PayPal did not confirm the authorization void.");
            }
            catch (Exception ex) { return (false, $"An error occurred while releasing the PayPal authorization: {ex.Message}"); }
        }

        private OrderRequest BuildAuthorizationRequestBody(decimal amount)
        {
            // Create the request body object
            var orderRequest = new OrderRequest()
            {
                // 1. 'Intent' is now set on the top-level OrderRequest object,
                // but its name has likely been changed for clarity.
                CheckoutPaymentIntent = "AUTHORIZE",

                // Create and configure the Purchase Unit
                PurchaseUnits = new List<PurchaseUnitRequest>()
        {
            new PurchaseUnitRequest()
            {
                // 2. 'Amount' is now a property of the PurchaseUnitRequest itself.
                AmountWithBreakdown = new AmountWithBreakdown()
                {
                    CurrencyCode = "USD",
                    Value = amount.ToString("F2")
                }
            }
        }
            };
            return orderRequest;
        }
    }
}