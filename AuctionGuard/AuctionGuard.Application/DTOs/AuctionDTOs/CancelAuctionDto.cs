using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.DTOs.AuctionDTOs
{
    /// <summary>
    /// Data transfer object for cancelling an auction.
    /// </summary>
    public class CancelAuctionDto
    {
        [Required]
        [MinLength(10, ErrorMessage = "Cancellation reason must be at least 10 characters long.")]
        public string Reason { get; set; }
    }
}
