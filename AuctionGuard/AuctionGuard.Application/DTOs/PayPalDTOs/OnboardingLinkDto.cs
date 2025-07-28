using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.DTOs.PayPalDTOs
{
    /// <summary>
    /// DTO to transfer the generated PayPal onboarding URL to the frontend.
    /// </summary>
    public class OnboardingLinkDto
    {
        public string OnboardingUrl { get; set; }
    }
}
