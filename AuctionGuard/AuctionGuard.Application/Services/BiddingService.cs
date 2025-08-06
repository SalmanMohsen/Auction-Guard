using AuctionGuard.Application.DTOs.BiddingDTOs;
using AuctionGuard.Application.IServices;
using AuctionGuard.Domain.Entities;
using AuctionGuard.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.Services
{
    public class BiddingService : IBiddingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IAuctionNotifier _notifier;
        private readonly ILogger<BiddingService> _logger;

        public BiddingService(IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            IAuctionNotifier notifier,
            ILogger<BiddingService> logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _notifier = notifier;
            _logger = logger;
        }

        public async Task<(bool success, string? error)> PlaceBidAsync(Guid auctionId, decimal amount, Guid userId)
        {
            _logger.LogInformation("------------------------------------------------------------------------- Starting PlaceBidAsync for Auction: {AuctionId}, User: {UserId} --------------------------------------------------------------------------------------------------", auctionId, userId);

            var auctionRepo = _unitOfWork.GetRepository<Auction>();
            var bidRepo = _unitOfWork.GetRepository<Bid>();
            var participantRepo = _unitOfWork.GetRepository<AuctionParticipant>();

            var auction = await auctionRepo.GetFirstOrDefaultAsync(
                predicate: a => a.AuctionId == auctionId,
                include: q => q.Include(a => a.Bids)
                               .Include(a => a.Property) 
            );

            if (auction == null)
            {
                _logger.LogWarning("Validation Failed: Auction not found.");
                return (false, "Auction not found.");
            }
            if (auction.Status != AuctionStatus.Active)
            {
                _logger.LogWarning("Validation Failed: Auction status is '{Status}', not Active.", auction.Status);
                return (false, "Auction is not active.");
            }
            if (auction.CreatorId == userId) return (false, "You cannot bid on your own auction.");

            var isParticipating = (await participantRepo.GetFirstOrDefaultAsync(p => p.AuctionId == auctionId && p.ParticipantId == userId)) != null;
            if (!isParticipating) return (false, "You are not a participant in this auction.");

            var highestBid = auction.Bids.Any() ? auction.Bids.Max(b => b.Amount) : auction.Property.PriceInitial;
            if (amount < highestBid + auction.MinBidIncrement)
            {
                return (false, $"Your bid must be at least {highestBid + auction.MinBidIncrement}.");
            }

            var bidder = await _userManager.FindByIdAsync(userId.ToString());
            if (bidder == null) return (false, "Bidder not found.");

            _logger.LogInformation("Initial auction validation passed.");

            var newBid = new Bid
            {
                AuctionId = auctionId,
                BidderId = userId,
                Amount = amount,
                Timestamp = DateTime.UtcNow
            };

            await bidRepo.AddAsync(newBid);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("Successfully saved new bid with Amount: {Amount}", newBid.Amount);

            var bidDto = new BidDto
            {
                BidId = newBid.BidId,
                AuctionId = newBid.AuctionId,
                BidderId = newBid.BidderId,
                BidderName = $"{bidder.FirstName} {bidder.LastName}",
                Amount = newBid.Amount,
                Timestamp = newBid.Timestamp
            };

            

            // Get all participants to notify them
            _logger.LogInformation("Sending notification to auction group {AuctionId}", auctionId);

            // Call the updated notifier method with just the auctionId
            await _notifier.NotifyNewBid(auctionId, bidDto);

            _logger.LogInformation("Notification sent successfully to group.");
            return (true, null);
        }
    }
}
