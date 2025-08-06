using AuctionGuard.Application.DTOs.AuctionParticipationDTOs;
using AuctionGuard.Application.IServices;
using AuctionGuard.Domain.Entities;
using AuctionGuard.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.Services
{
    public class AuctionParticipationService : IAuctionParticipationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPayPalClientService _payPalClient;
        private readonly IGenericRepository<Auction> _auctionRepo;
        private readonly IGenericRepository<AuctionParticipant> _participantRepo;
        private readonly IGenericRepository<BidderAuthorization> _bidderAuthRepo;
        private readonly IMemoryCache _cache;

        public AuctionParticipationService(
            IUnitOfWork unitOfWork,
            IPayPalClientService payPalClient,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _payPalClient = payPalClient;
            _auctionRepo = _unitOfWork.GetRepository<Auction>();
            _participantRepo = _unitOfWork.GetRepository<AuctionParticipant>();
            _bidderAuthRepo = _unitOfWork.GetRepository<BidderAuthorization>();
            _cache = cache;
        }

        public async Task<(JoinAuctionResponseDto? response, string? error)> InitiateJoinProcessAsync(Guid auctionId, Guid userId)
        {
            var auction = await _auctionRepo.GetByIdAsync(auctionId);
            if (auction == null) return (null, "Auction not found.");
            if (auction.Status != AuctionStatus.Scheduled && auction.Status != AuctionStatus.Active) return (null, "You can only join scheduled or active auctions.");
            if (auction.CreatorId == userId) return (null, "Sellers cannot participate in their own auctions.");

            var isParticipant = await _participantRepo.FindAsync(p => p.AuctionId == auctionId && p.ParticipantId == userId);
            if (isParticipant != null) return (null, "You are already participating in this auction.");

            if (auction.GuaranteeDeposit <= 0)
            {
                await _participantRepo.AddAsync(new AuctionParticipant { AuctionId = auctionId, ParticipantId = userId, RegistrationTime = DateTime.UtcNow });
                await _unitOfWork.CommitAsync();
                return (new JoinAuctionResponseDto { ApprovalUrl = "JOINED_NO_DEPOSIT" }, "Successfully joined auction. No deposit was required.");
            }

            
            var (orderId, approvalUrl, error) = await _payPalClient.CreateDepositOrderAsync(auction.GuaranteeDeposit, "USD", auctionId, userId);

            // If there's an error, it will now contain the detailed reason from PayPal.
            if (!string.IsNullOrEmpty(error))
            {
                // Log the full 'error' content for debugging purposes
                // Return a user-friendly message, but the logged error is key
                return (null, $"Failed to create PayPal order. Please try again. (Details: {error})");
            }
            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(approvalUrl))
            {
                return (null, "Failed to create PayPal order for an unknown reason.");
            }


            var cacheEntry = new PayPalOrderCacheDto { AuctionId = auctionId, UserId = userId };
            _cache.Set(orderId, cacheEntry, TimeSpan.FromMinutes(10));
            return (new JoinAuctionResponseDto { ApprovalUrl = approvalUrl }, null);
        }

        public async Task<(bool success, string? error)> ConfirmJoinProcessAsync(string payPalOrderId, Guid userId, Guid auctionId)
        {
            var (authorizationId, error) = await _payPalClient.AuthorizeDepositOrderAsync(payPalOrderId);
            if (!string.IsNullOrEmpty(error))
            {
                return (false, $"Failed to authorize payment: {error}");
            }

            var auction = await _auctionRepo.GetByIdAsync(auctionId);
            if (auction == null) return (false, "Auction not found.");

            var authorization = new BidderAuthorization
            {
                UserId = userId,
                AuctionId = auctionId,
                AmountHeld = auction.GuaranteeDeposit,
                GatewayAuthId = authorizationId,
                Status = BiddingAuthStatus.active,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(29) // PayPal authorizations expire in 29 days
            };
            await _bidderAuthRepo.AddAsync(authorization);

            var participant = new AuctionParticipant
            {
                AuctionId = auctionId,
                ParticipantId = userId,
                RegistrationTime = DateTime.UtcNow
            };
            await _participantRepo.AddAsync(participant);

            await _unitOfWork.CommitAsync();
            return (true, "Successfully joined auction and placed hold on deposit.");
        }

        public async Task<(bool success, string? error)> LeaveAuctionAsync(Guid auctionId, Guid userId)
        {
            var auction = await _auctionRepo.GetByIdAsync(auctionId);
            if (auction == null) return (false, "Auction not found.");
            if (auction.Status != AuctionStatus.Scheduled) return (false, "You can only leave a scheduled auction.");

            var participant = await _participantRepo.FindAsync(p => p.AuctionId == auctionId && p.ParticipantId == userId);
            if (participant == null) return (false, "You are not a participant in this auction.");

            var authorization = await _bidderAuthRepo.GetFirstOrDefaultAsync(a => a.AuctionId == auctionId && a.UserId == userId && a.Status == BiddingAuthStatus.active);
            if (authorization != null)
            {
                var released = await _payPalClient.VoidDepositAuthorizationAsync(authorization.GatewayAuthId);
                if (released)
                {
                    authorization.Status = BiddingAuthStatus.released;
                    _bidderAuthRepo.Update(authorization);
                }
                else
                {
                    // Critical failure - needs logging and possibly manual intervention
                    return (false, "Could not release the hold on your funds. Please contact support.");
                }
            }

            _participantRepo.Remove(participant);
            await _unitOfWork.CommitAsync();
            return (true, "You have successfully left the auction and the hold has been released.");
        }

        public async Task<ParticipationStatusDto> CheckParticipationStatusAsync(Guid auctionId, Guid userId)
        {
            var isParticipant = await _participantRepo.FindAsync(p => p.AuctionId == auctionId && p.ParticipantId == userId);
            return new ParticipationStatusDto
            {
                IsParticipant = isParticipant != null,
                StatusMessage = isParticipant != null ? "You are participating in this auction." : "You are not participating in this auction."
            };
        }
    }
}
