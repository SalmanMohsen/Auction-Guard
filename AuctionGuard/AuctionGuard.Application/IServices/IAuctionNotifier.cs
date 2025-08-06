using AuctionGuard.Application.DTOs.BiddingDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.IServices
{
    public interface IAuctionNotifier
    {
        Task NotifyAuctionStarted(List<string> participantIds, DateTime endDate);
        Task NotifyAuctionFinished(List<string> participantIds);
        Task NotifyNewBid(Guid auctionId, BidDto bid);
    }
}
