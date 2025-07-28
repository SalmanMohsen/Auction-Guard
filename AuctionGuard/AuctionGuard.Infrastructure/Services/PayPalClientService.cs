using AuctionGuard.Application.IServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace AuctionGuard.Infrastructure.Services
{
    public class PayPalClientService : IPayPalClientService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _platformClientId;
        private readonly string _platformClientSecret;

        private readonly string _merchantClientId;
        private readonly string _merchantClientSecret;

        private readonly string _redirectUri;
        private readonly string _mode;

        public PayPalClientService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _platformClientId = _configuration["PayPal:ClientId"];
            _platformClientSecret = _configuration["PayPal:ClientSecret"];
            _redirectUri = _configuration["PayPal:RedirectUri"];
            _mode = _configuration["PayPal:Mode"];

            _merchantClientId = _configuration["PayPal:Merchant:ClientId"];
            _merchantClientSecret = _configuration["PayPal:Merchant:ClientSecret"];
        }

        public string GenerateOnboardingAuthorizationUrl()
        {
            // We now construct the URL with the correct base for web flows, not the API.
            var authBaseUrl = _mode == "Sandbox"
                ? "https://www.sandbox.paypal.com"
                : "https://www.paypal.com";

            var scopes = "https://uri.paypal.com/services/invoicing https://uri.paypal.com/services/paypalhere";
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["client_id"] = _platformClientId;
            query["response_type"] = "code";
            query["scope"] = scopes;
            query["redirect_uri"] = _redirectUri;

            return $"{authBaseUrl}/webapps/auth/protocol/openidconnect/v1/authorize?{query}";
        }

        public async Task<(string? merchantId, string? error)> GetMerchantIdFromAuthCode(string authorizationCode)
        {
            var accessToken = await GetAccessTokenFromAuthCode(authorizationCode);
            if (string.IsNullOrEmpty(accessToken))
            {
                return (null, "Failed to get access token from authorization code.");
            }

            var request = new HttpRequestMessage(HttpMethod.Get, "/v1/identity/oauth2/userinfo?schema=paypalv1.1");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                // Log errorContent for debugging
                return (null, "Failed to retrieve user info from PayPal.");
            }

            var responseData = await response.Content.ReadFromJsonAsync<JsonElement>();
            var merchantId = responseData.GetProperty("payer_id").GetString();

            return (merchantId, null);
        }

        private async Task<string?> GetAccessTokenFromAuthCode(string authorizationCode)
        {
            var bytes = Encoding.UTF8.GetBytes($"{_platformClientId}:{_platformClientSecret}");
            var request = new HttpRequestMessage(HttpMethod.Post, "/v1/identity/openidconnect/tokenservice")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"grant_type", "authorization_code"},
                    {"code", authorizationCode},
                    {"redirect_uri", _redirectUri } // It's good practice to include the redirect_uri here as well
                })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                // Log errorContent for debugging
                return null;
            }

            var responseData = await response.Content.ReadFromJsonAsync<JsonElement>();
            return responseData.GetProperty("access_token").GetString();
        }

        public async Task<(string? orderId, string? approvalUrl, string? error)> CreateDepositOrderAsync(decimal amount, string currency, Guid auctionId, Guid userId)
        {
            var accessToken = await GetAccessToken(_merchantClientId, _merchantClientSecret);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var depositRedirects = GetDepositRedirectUrls();

            var orderRequest = new
            {
                intent = "AUTHORIZE",
                purchase_units = new[]
                {
                    new
                    {
                        description = $"Guarantee Deposit for Auction ID: {auctionId}",
                        amount = new { currency_code = currency, value = amount.ToString("F2") },
                        custom_id = $"user:{userId}|auction:{auctionId}"
                    }
                },
                application_context = new
                {
                    return_url = depositRedirects.returnUrl,
                    cancel_url = depositRedirects.cancelUrl,
                    brand_name = "AuctionGuard",
                    shipping_preference = "NO_SHIPPING"
                }
            };

            var response = await _httpClient.PostAsJsonAsync("/v2/checkout/orders", orderRequest);

            // If the call fails, read and return the error message from PayPal.
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                // errorContent will contain a JSON object from PayPal with specific details.
                return (null, null, errorContent);
            }

            var responseData = await response.Content.ReadFromJsonAsync<JsonElement>();
            var orderId = responseData.GetProperty("id").GetString();
            var approvalUrl = responseData.GetProperty("links").EnumerateArray()
                                .First(link => link.GetProperty("rel").GetString() == "approve")
                                .GetProperty("href").GetString();

            return (orderId, approvalUrl, null); // Return null for the error on success.
        }

        public async Task<(string? authorizationId, string? error)> AuthorizeDepositOrderAsync(string orderId)
        {
            var accessToken = await GetAccessToken(_merchantClientId, _merchantClientSecret);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.PostAsJsonAsync($"/v2/checkout/orders/{orderId}/authorize", new { });
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return (null, errorContent);
            }

            var responseData = await response.Content.ReadFromJsonAsync<JsonElement>();
            var authorizationId = responseData.GetProperty("purchase_units")[0]
                                    .GetProperty("payments")
                                    .GetProperty("authorizations")[0]
                                    .GetProperty("id")
                                    .GetString();
            return (authorizationId, null);
        }

        public async Task<bool> VoidDepositAuthorizationAsync(string authorizationId)
        {
            var accessToken = await GetAccessToken(_merchantClientId, _merchantClientSecret);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.PostAsync($"/v2/payments/authorizations/{authorizationId}/void", null);
            return response.IsSuccessStatusCode; // 204 No Content on success
        }

        // --- Helper Methods ---
        private (string returnUrl, string cancelUrl) GetDepositRedirectUrls()
        {
            var baseUrl = _configuration["ApplicationBaseUrl"]; // e.g., "https://localhost:7044"
            return (
                $"{baseUrl}/api/auctions/join/success",
                $"{baseUrl}/api/auctions/join/cancel"
            );
        }

        private async Task<string> GetAccessToken(string clientId, string clientSecret)
        {
            var bytes = Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
            var request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token")
            {
                Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadFromJsonAsync<JsonElement>();
            return responseData.GetProperty("access_token").GetString();
        }
    }
}
