using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.DTOs.BiddingDTOs
{
    public class BidDto
    {
        public Guid BidId { get; set; }
        public Guid AuctionId { get; set; }
        public Guid BidderId { get; set; }
        public string BidderName { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
