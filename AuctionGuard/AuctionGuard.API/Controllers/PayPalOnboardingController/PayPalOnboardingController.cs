using AuctionGuard.Application.Authorization;
using AuctionGuard.Application.IServices;
using AuctionGuard.Infrastructure.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuctionGuard.API.Controllers.PayPalOnboardingController
{
    [Route("api/paypal-onboarding")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PayPalOnboardingController : ControllerBase
    {
        private readonly IPayPalOnboardingService _onboardingService;
        private readonly IPayPalClientService _payPalClientService;

        public PayPalOnboardingController(IPayPalOnboardingService onboardingService, IPayPalClientService payPalClientService)
        {
            _onboardingService = onboardingService;
            _payPalClientService = payPalClientService;
        }

        /// <summary>
        /// Generates a link for the current user to begin the PayPal seller onboarding process.
        /// </summary>
        [HttpGet("generate-link")]
        [Authorize(Roles ="Seller")]
        public IActionResult GenerateOnboardingLink()
        {
            // It now calls the client service directly because no user data is needed to build the URL.
            var onboardingUrl = _payPalClientService.GenerateOnboardingAuthorizationUrl();
            return Ok(new { onboardingUrl });
        }

        /// <summary>
        /// Checks the PayPal onboarding status for the current user.
        /// </summary>
        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            var status = await _onboardingService.GetOnboardingStatusAsync(userId);
            return Ok(status);
        }

        /// <summary>
        /// The callback URL that PayPal redirects to after a seller grants consent.
        /// </summary>
        [HttpGet("callback")]
        [AllowAnonymous] // Still must be anonymous to receive the redirect
        public async Task<IActionResult> PayPalCallback([FromQuery] string code)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Redirect("https://your-frontend.com/login?error=session_expired_for_onboarding");
            }

            if (string.IsNullOrEmpty(code))
            {
                return Redirect("https://your-frontend.com/onboarding-failed?error=paypal_did_not_return_code");
            }

            // Exchange the code for the merchant ID
            var (merchantId, error) = await _payPalClientService.GetMerchantIdFromAuthCode(code);

            if (error != null || string.IsNullOrEmpty(merchantId))
            {
                return Redirect($"https://your-frontend.com/onboarding-failed?error={error ?? "could_not_get_merchant_id"}");
            }

            // Complete the onboarding process in your database
            var (success, dbError) = await _onboardingService.CompleteOnboardingAsync(userId, merchantId);

            if (!success)
            {
                return Redirect($"https://your-frontend.com/onboarding-failed?error={dbError}");
            }

            return Redirect("https://your-frontend.com/onboarding-success");
        }
    }
}
