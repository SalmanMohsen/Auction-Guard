using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionGuard.Application.DTOs.OfferDTO;

namespace AuctionGuard.Application.DTOs.AuctionDTOs
{
    /// <summary>
    /// Data transfer object for updating the core details of an auction.
    /// Note: Offers are managed via the OfferService, not here.
    /// </summary>
    public class UpdateAuctionDto
    {
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? MinBidIncrement { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? GuaranteeDeposit { get; set; }
    }
}
