using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.DTOs.AuctionParticipationDTOs
{
    /// <summary>
    /// DTO to check if a user is already a participant in an auction.
    /// </summary>
    public class ParticipationStatusDto
    {
        public bool IsParticipant { get; set; }
        public string? StatusMessage { get; set; }
    }

}
