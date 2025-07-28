using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.DTOs.OfferDTO
{
    public class OfferDto
    {
        public Guid OfferId { get; set; }
        public string Description { get; set; }
        public decimal TriggerPrice { get; set; }
    }
}
