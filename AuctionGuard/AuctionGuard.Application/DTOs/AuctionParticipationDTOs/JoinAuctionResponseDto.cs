using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.DTOs.AuctionParticipationDTOs
{
    /// <summary>
    /// DTO to send the PayPal approval URL back to the frontend.
    /// </summary>
    public class JoinAuctionResponseDto
    {
        public string ApprovalUrl { get; set; }
    }
}
