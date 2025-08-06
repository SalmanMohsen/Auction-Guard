using AuctionGuard.API.Hubs;
using AuctionGuard.Application.DTOs.BiddingDTOs;
using AuctionGuard.Application.IServices;
using Microsoft.AspNetCore.SignalR;

namespace AuctionGuard.API.Services
{
    public class SignalRNotifier : IAuctionNotifier
    {
        private readonly IHubContext<BiddingHub> _hubContext;

        public SignalRNotifier(IHubContext<BiddingHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyAuctionStarted(List<string> participantIds, DateTime endDate)
        {
            await _hubContext.Clients.Users(participantIds).SendAsync("AuctionStarted", endDate);
        }

        public async Task NotifyAuctionFinished(List<string> participantIds)
        {
            await _hubContext.Clients.Users(participantIds).SendAsync("AuctionFinished");
        }

        public async Task NotifyNewBid(Guid auctionId, BidDto bid)
        {
            await _hubContext.Clients.Group(auctionId.ToString()).SendAsync("ReceiveNewBid", bid);
        }
    }
}
