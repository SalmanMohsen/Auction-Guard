using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.DTOs.AuctionParticipationDTOs
{
    /// <summary>
    /// Represents the request from the frontend after the user has approved the transaction in the PayPal popup.
    /// </summary>
    public class ConfirmParticipationRequestDto
    {
        [Required]
        public string PayPalOrderId { get; set; }
    }
}
