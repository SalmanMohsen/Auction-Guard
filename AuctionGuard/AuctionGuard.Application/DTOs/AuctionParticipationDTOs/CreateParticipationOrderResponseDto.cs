using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.DTOs.AuctionParticipationDTOs
{
    /// <summary>
    /// Represents the response after creating a PayPal order for participation.
    /// This ID is sent to the frontend to be used with the PayPal JavaScript SDK.
    /// </summary>
    public class CreateParticipationOrderResponseDto
    {
        public string PayPalOrderId { get; set; }
    }
}
