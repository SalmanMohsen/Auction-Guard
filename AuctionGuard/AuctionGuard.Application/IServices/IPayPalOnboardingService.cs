using AuctionGuard.Application.DTOs.PayPalDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.IServices
{
    /// <summary>
    /// Defines the contract for handling the PayPal seller onboarding process.
    /// </summary>
    public interface IPayPalOnboardingService
    {
        

        /// <summary>
        /// Finalizes the onboarding process after the user is redirected back from PayPal.
        /// </summary>
        /// <param name="userId">The user who is being onboarded.</param>
        /// <param name="payPalMerchantId">The merchant ID returned from PayPal.</param>
        /// <returns>A result indicating success or failure.</returns>
        Task<(bool success, string? error)> CompleteOnboardingAsync(Guid userId, string payPalMerchantId);

        /// <summary>
        /// Checks if a user has already completed the PayPal onboarding process.
        /// </summary>
        /// <param name="userId">The ID of the user to check.</param>
        /// <returns>A DTO with the user's onboarding status.</returns>
        Task<PayPalOnboardingStatusDto> GetOnboardingStatusAsync(Guid userId);
    }
}
