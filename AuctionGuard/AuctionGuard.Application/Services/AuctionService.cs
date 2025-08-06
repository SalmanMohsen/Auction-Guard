using AuctionGuard.Application.DTOs.AuctionDTOs;
using AuctionGuard.Application.DTOs.OfferDTO;
using AuctionGuard.Application.IServices;
using AuctionGuard.Domain.Entities;
using AuctionGuard.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionGuard.Application.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Auction> _auctionRepo;
        private readonly IGenericRepository<Property> _propertyRepo;

        public AuctionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _auctionRepo = _unitOfWork.GetRepository<Auction>();
            _propertyRepo = _unitOfWork.GetRepository<Property>();
        }

        
        public async Task<(AuctionDto? auction, string? error)> CreateAuctionAsync(CreateAuctionDto dto, Guid sellerId)
        {
            var property = await _propertyRepo.GetByIdAsync(dto.PropertyId);

            if (property == null) return (null, "Property not found.");
            if (property.OwnerId != sellerId) return (null, "You are not the owner of this property.");
            if (property.ApprovalStatus != ApprovalStatus.Approved) return (null, "Property must be approved before an auction can be created.");

            var existingAuctions = await _auctionRepo.FindAllByPredicateAsync(a => a.PropertyId == dto.PropertyId && (a.Status == AuctionStatus.Active || a.Status == AuctionStatus.Scheduled));
            if (existingAuctions.Any())
            {
                return (null, "This property is already in an active or scheduled auction.");
            }
            TimeZoneInfo localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Syria Standard Time");
            var auction = new Auction
            {
                PropertyId = dto.PropertyId,
                CreatorId = sellerId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                MinBidIncrement = dto.MinBidIncrement,
                GuaranteeDeposit = dto.GuaranteeDeposit,
                CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, localTimeZone),
                Status = dto.StartTime <= TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, localTimeZone) ? AuctionStatus.Active : AuctionStatus.Scheduled
            };
            property.PropertyStatus = PropertyStatus.UnderAuction;
            if (dto.Offers != null)
            {
                foreach (var offerDto in dto.Offers)
                {
                    auction.Offers.Add(new Offer
                    {
                        Description = offerDto.Description,
                        TriggerPrice = offerDto.TriggerPrice
                    });
                }
            }
            _propertyRepo.Update(property);
            await _auctionRepo.AddAsync(auction);
            await _unitOfWork.CommitAsync();

            return (await MapToAuctionDto(auction), null);
        }


        public async Task<IEnumerable<AuctionDto>> GetSellerAuctionsAsync(Guid sellerId)
        {
            var auctions = await _auctionRepo.FindAllByPredicateAsync(
                predicate: a => a.CreatorId == sellerId,
                include: q => q.Include(a => a.Property)
                               .Include(a => a.Offers)
                               .Include(a => a.Bids)
            );
            return await MapToAuctionDtoList(auctions);
        }

        public async Task<IEnumerable<AuctionDto>> GetMyWonAuctionsAsync(Guid userId)
        {
            var auctions = await _auctionRepo.FindAllByPredicateAsync(
                predicate: a => a.WinnerId == userId,
                include: q => q.Include(a => a.Property)
                               .Include(a => a.Offers)
                               .Include(a => a.Bids)
            );
            return await MapToAuctionDtoList(auctions);
        }

        
        public async Task<IEnumerable<AuctionDto>> GetActiveAuctionsAsync()
        {
            var auctions = await _auctionRepo.FindAllByPredicateAsync(
                predicate: a => a.Status == AuctionStatus.Active,
                include: q => q.Include(a => a.Property)
                               .Include(a => a.Offers)
                               .Include(a => a.Bids)
            );
            return await MapToAuctionDtoList(auctions);
        }
        public async Task<IEnumerable<AuctionDto>> GetScheduledAuctionsAsync()
        {
            var auctions = await _auctionRepo.FindAllByPredicateAsync(
                predicate: a => a.Status == AuctionStatus.Scheduled,
                include: q => q.Include(a => a.Property)
                               .Include(a => a.Offers)
                               .Include(a => a.Bids)
            );
            return await MapToAuctionDtoList(auctions);
        }
        public async Task<IEnumerable<AuctionDto>> GetEndedAuctionsAsync()
        {
            var auctions = await _auctionRepo.FindAllByPredicateAsync(
                predicate: a => a.Status == AuctionStatus.Ended,
                include: q => q.Include(a => a.Property)
                               .Include(a => a.Offers)
                               .Include(a => a.Bids)
            );
            return await MapToAuctionDtoList(auctions);
        }
        public async Task<IEnumerable<AuctionDto>> GetCancelledAuctionsAsync()
        {
            var auctions = await _auctionRepo.FindAllByPredicateAsync(
                predicate: a => a.Status == AuctionStatus.Cancelled,
                include: q => q.Include(a => a.Property)
                               .Include(a => a.Offers)
                               .Include(a => a.Bids)
            );
            return await MapToAuctionDtoList(auctions);
        }
        public async Task<AuctionDto?> GetAuctionByIdAsync(string id)
        {
            var auction = await _auctionRepo.GetFirstOrDefaultAsync(
                predicate: a => a.AuctionId.ToString() == id,
                include: q => q.Include(a => a.Property)
                               .Include(a => a.Offers)
                               .Include(a => a.Bids)
            );
            return auction == null ? null : await MapToAuctionDto(auction);
        }

        
        public async Task<(AuctionDto? auction, string? error)> UpdateAuctionAsync(Guid auctionId, UpdateAuctionDto dto, Guid userId)
        {
            var auction = await _auctionRepo.GetByIdAsync(auctionId);

            if (auction == null) return (null, "Auction not found.");
            if (auction.CreatorId != userId) return (null, "You are not authorized to update this auction.");
            if (auction.Status != AuctionStatus.Scheduled) return (null, "Only scheduled auctions can be updated.");

            auction.StartTime = dto.StartTime ?? auction.StartTime;
            auction.EndTime = dto.EndTime ?? auction.EndTime;
            auction.MinBidIncrement = dto.MinBidIncrement ?? auction.MinBidIncrement;
            auction.GuaranteeDeposit = dto.GuaranteeDeposit ?? auction.GuaranteeDeposit;

            _auctionRepo.Update(auction);
            await _unitOfWork.CommitAsync();

            return (await MapToAuctionDto(auction), null);
        }

        public async Task<(bool success, string? error)> DeleteAuctionAsync(Guid auctionId, Guid userId, bool isAdmin)
        {
            var auction = await _auctionRepo.GetByIdAsync(auctionId);
            var property = await _propertyRepo.GetByIdAsync(auction.PropertyId);
            if (auction == null) return (true, null); 
            if (auction.Status != AuctionStatus.Scheduled) return (false, "Only scheduled auctions can be deleted.");
            if(auction.CreatorId != userId && !isAdmin)
            {
                return (false, "You are not allowed to delete the auction.");
            }
            property.PropertyStatus = PropertyStatus.Available;
            _propertyRepo.Update(property);
            _auctionRepo.Remove(auction);
            await _unitOfWork.CommitAsync();
            return (true, null);
        }
        
        public async Task<(bool success, string? error)> AttemptRemakeAuctionAsync(Guid auctionId, Guid sellerId)
        {
            var auction = await _auctionRepo.GetFirstOrDefaultAsync(a => a.AuctionId == auctionId, q => q.Include(a => a.Bids));
            if (auction == null || auction.CreatorId != sellerId) return (false, "Auction not found or you are not the creator.");
            if (auction.Status != AuctionStatus.Ended) return (false, "Only ended auctions can be remade.");
            if (auction.Bids.Any()) return (false, "Cannot remake an auction that had bids.");

            var property = await _propertyRepo.GetByIdAsync(auction.PropertyId);
            if (property == null) return (false, "Associated property could not be found.");

            property.PropertyStatus = PropertyStatus.Available;
            _propertyRepo.Update(property);

            auction.Status = AuctionStatus.Scheduled;
            _auctionRepo.Update(auction);

            await _unitOfWork.CommitAsync();
            return (true, "Auction has been rescheduled and the property is available again.");
        }


        


        public async Task<(bool success, string? error)> CancelAuctionAsync(Guid auctionId, Guid userId, bool isAdmin, string reason)
        {
            var auction = await _auctionRepo.GetByIdAsync(auctionId);

            if (auction == null) return (false, "Auction not found.");

            if (auction.CreatorId != userId && !isAdmin)
            {
                return (false, "You are not authorized to cancel this auction.");
            }

            if (auction.Status != AuctionStatus.Scheduled && auction.Status != AuctionStatus.Active)
            {
                return (false, "Only scheduled or active auctions can be canceled.");
            }

            var property = await _propertyRepo.GetByIdAsync(auction.PropertyId);
            if (property == null) return (false, "Associated property could not be found.");

            // Update auction state
            auction.Status = AuctionStatus.Cancelled;
            auction.CancellationReason = reason;
            _auctionRepo.Update(auction);

            // Make the property available again
            property.PropertyStatus = PropertyStatus.Available;
            _propertyRepo.Update(property);

            await _unitOfWork.CommitAsync();

            return (true, "Auction has been successfully cancelled.");
        }


        // Helper to map a list of auctions
        private async Task<List<AuctionDto>> MapToAuctionDtoList(IEnumerable<Auction> auctions)
        {
            var dtoList = new List<AuctionDto>();
            foreach (var auction in auctions)
            {
                dtoList.Add(await MapToAuctionDto(auction));
            }
            return dtoList;
        }

        // Central mapping logic
        private async Task<AuctionDto> MapToAuctionDto(Auction auction)
        {
            // The Property is loaded via the .Include() in the calling methods.
            var property = auction.Property;

            if (auction.Offers == null || !auction.Offers.Any())
            {
                var offerRepo = _unitOfWork.GetRepository<Offer>();
                auction.Offers = (await offerRepo.FindAllByPredicateAsync(o => o.AuctionId == auction.AuctionId)).ToList();
            }

            return new AuctionDto
            {
                AuctionId = auction.AuctionId,
                StartTime = auction.StartTime,
                EndTime = auction.EndTime,
                MinBidIncrement = auction.MinBidIncrement,
                CurrentHighestBid = auction.Bids.Any() ? auction.Bids.Max(b => b.Amount) : (decimal?)null,
                GuaranteeDeposit = auction.GuaranteeDeposit,
                Status = auction.Status,
                PropertyId = auction.PropertyId,
                PropertyTitle = property?.Title,
                PropertyDescription = property?.Description, // Map the description
                CreatorId = auction.CreatorId,
                WinnerId = auction.WinnerId,
                CancellationReason = auction.CancellationReason,
                Offers = auction.Offers.Select(o => new OfferDto { OfferId = o.OfferId, Description = o.Description, TriggerPrice = o.TriggerPrice }).ToList(),
                PropertyImageUrls = property?.ImageUrls ?? new List<string>() // Map the image URLs
            };
        }
    }
}
