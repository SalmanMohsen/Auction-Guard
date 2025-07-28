using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.DTOs.PayPalDTOs
{
    /// <summary>
    /// DTO to inform the frontend about the user's current PayPal onboarding status.
    /// </summary>
    public class PayPalOnboardingStatusDto
    {
        public bool IsOnboarded { get; set; }
        public string? StatusMessage { get; set; }
    }
}
