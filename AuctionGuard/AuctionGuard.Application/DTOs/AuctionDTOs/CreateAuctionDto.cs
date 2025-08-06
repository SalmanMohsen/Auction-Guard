using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionGuard.Application.DTOs.OfferDTO;

namespace AuctionGuard.Application.DTOs.AuctionDTOs
{
    public class CreateAuctionDto
    {
        [Required]
        public Guid PropertyId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal MinBidIncrement { get; set; }

        [Range(0, double.MaxValue)]
        public decimal GuaranteeDeposit { get; set; }

        public List<CreateOfferDto> Offers { get; set; } = new List<CreateOfferDto>();
    }
}
