using AuctionGuard.Application.DTOs.PayPalDTOs;
using AuctionGuard.Application.IServices;
using AuctionGuard.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.Services
{
    public class PayPalOnboardingService : IPayPalOnboardingService
    {
        private readonly UserManager<User> _userManager;
        // This service is no longer used here for generating the link,
        // but we'll keep it for the CompleteOnboardingAsync logic if needed elsewhere.
        private readonly IPayPalClientService _payPalClient;

        public PayPalOnboardingService(UserManager<User> userManager, IPayPalClientService payPalClient)
        {
            _userManager = userManager;
            _payPalClient = payPalClient;
        }

        // THIS METHOD IS NOW REMOVED as the controller will call the client service directly.
        // The logic to generate the link is simple URL construction and doesn't need
        // the extra layer of this service. The controller can handle it.
        /*
        public async Task<(OnboardingLinkDto? link, string? error)> GenerateOnboardingLinkAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return (null, "User not found.");
            }
            if (!string.IsNullOrEmpty(user.PayPalMerchantId))
            {
                return (null, "This account has already been onboarded.");
            }
            // *** THIS IS THE LOGIC THAT IS NOW HANDLED DIRECTLY IN THE CONTROLLER & CLIENT ***
            var onboardingUrl = _payPalClient.GenerateOnboardingAuthorizationUrl();
            var linkDto = new OnboardingLinkDto { OnboardingUrl = onboardingUrl };
            return (linkDto, null);
        }
        */

        public async Task<(bool success, string? error)> CompleteOnboardingAsync(Guid userId, string payPalMerchantId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return (false, "User not found.");
            }

            // Prevent overwriting an existing ID unless that is desired business logic
            if (!string.IsNullOrEmpty(user.PayPalMerchantId))
            {
                return (false, "This user has already been onboarded with a PayPal account.");
            }

            user.PayPalMerchantId = payPalMerchantId;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                // It's helpful to log result.Errors here
                return (false, "Failed to update user with PayPal merchant ID.");
            }

            return (true, "Successfully onboarded PayPal seller.");
        }

        public async Task<PayPalOnboardingStatusDto> GetOnboardingStatusAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var isOnboarded = user != null && !string.IsNullOrEmpty(user.PayPalMerchantId);

            return new PayPalOnboardingStatusDto
            {
                IsOnboarded = isOnboarded,
                StatusMessage = isOnboarded ? "This account is ready to receive payments." : "This account has not been connected to PayPal."
            };
        }
    }
}
