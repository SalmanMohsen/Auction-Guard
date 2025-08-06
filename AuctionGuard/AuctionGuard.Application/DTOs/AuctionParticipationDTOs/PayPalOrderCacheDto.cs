using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.DTOs.AuctionParticipationDTOs
{
    /// <summary>
    /// A strongly-typed object for storing temporary data related to a PayPal order in the cache.
    /// This avoids the use of 'dynamic' and prevents runtime binding errors.
    /// </summary>
    public class PayPalOrderCacheDto
    {
        public Guid AuctionId { get; set; }
        public Guid UserId { get; set; }
    }
}
