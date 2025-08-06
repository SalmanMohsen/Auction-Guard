using AuctionGuard.Application.IServices;
using AuctionGuard.Domain.Entities;
using AuctionGuard.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace AuctionGuard.API.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public sealed class BiddingHub : Hub
    {
        private readonly IBiddingService _biddingService;
        private readonly IAuctionParticipationService _participationService;
        private readonly IGenericRepository<Auction> _auctionRepo;
        private readonly ILogger<BiddingHub> _logger;

        public BiddingHub(
            IBiddingService biddingService,
            IAuctionParticipationService participationService,
            IGenericRepository<Auction> auctionRepo,
            ILogger<BiddingHub> logger)
        {
            _biddingService = biddingService;
            _participationService = participationService;
            _auctionRepo = auctionRepo;
            _logger = logger; 
        }
        public async Task Ping(string message)
        {
            // This is the log we are trying to see
            _logger.LogInformation("-------------------------------------------- TEST HUB PING RECEIVED: {Message} -------------------------------------", message);

            // Send a message back to the caller to confirm
            await Clients.All.SendAsync("Pong", $"Server has received your message: {message}");
        }

        public async Task PlaceBid( Guid auctionId, decimal amount)
        {
            _logger.LogInformation("--- Hub Method 'PlaceBid' invoked by User: {UserId} for Auction: {AuctionId} ---", Context.UserIdentifier, auctionId);

            try
            {
                var userId = Guid.Parse(Context.UserIdentifier);
                var (success, error) = await _biddingService.PlaceBidAsync(auctionId, amount, userId);

                if (!success)
                {
                    _logger.LogWarning("Bidding service returned failure for User: {UserId}. Error: {Error}", userId, error);
                    await Clients.Caller.SendAsync("BiddingError", error);
                }
                else
                {
                    _logger.LogInformation("Bidding service returned success for User: {UserId}.", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred in PlaceBid hub method for User: {UserId}", Context.UserIdentifier);
                await Clients.Caller.SendAsync("BiddingError", "A server error occurred while placing your bid.");
            }
        }

        public async Task JoinAuctionGroup(Guid auctionId)
        {
            var userId = Guid.Parse(Context.UserIdentifier);
            var participating = await _participationService.CheckParticipationStatusAsync(auctionId, userId);

            if (participating.IsParticipant)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, auctionId.ToString());
                await Clients.Caller.SendAsync("JoinSuccess", $"You have successfully joined the group for auction {auctionId}.");
            }
            else
            {
                await Clients.Caller.SendAsync("JoinFailed", participating.StatusMessage ?? "You are not an authorized participant for this auction.");
            }
        }

        public async Task LeaveAuctionGroup(Guid auctionId)
        {
            var auction = await _auctionRepo.GetFirstOrDefaultAsync(predicate: a => a.AuctionId == auctionId);
            if (auction != null && auction.Status == Domain.Entities.AuctionStatus.Active)
            {
                await Clients.Caller.SendAsync("LeaveGroupFailed", "You cannot leave the group of an active auction.");
                return;

            }
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, auctionId.ToString());
            await Clients.Caller.SendAsync("LeaveGroupSuccess", "You have successfully left the auction group.");
        }

        
    }
}
