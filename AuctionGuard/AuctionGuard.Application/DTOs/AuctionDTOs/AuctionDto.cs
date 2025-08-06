using AuctionGuard.Application.DTOs.OfferDTO;
using AuctionGuard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.DTOs.AuctionDTOs
{
    public class AuctionDto
    {
        public Guid AuctionId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal MinBidIncrement { get; set; }
        public decimal? CurrentHighestBid { get; set; }
        public decimal GuaranteeDeposit { get; set; }
        public AuctionStatus Status { get; set; }
        public Guid PropertyId { get; set; }
        public string PropertyTitle { get; set; }
        public string PropertyDescription { get; set; }
        public Guid CreatorId { get; set; }
        public Guid? WinnerId { get; set; }
        public string? CancellationReason { get; set; }
        public List<OfferDto> Offers { get; set; }
        public List<string> PropertyImageUrls { get; set; } = new List<string>();
    }
}
