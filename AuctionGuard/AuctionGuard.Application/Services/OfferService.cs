using AuctionGuard.Application.DTOs.OfferDTO;
using AuctionGuard.Application.IServices;
using AuctionGuard.Domain.Entities;
using AuctionGuard.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.Services
{
    public class OfferService : IOfferService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Auction> _auctionrepo;
        private readonly IGenericRepository<Offer> _offerrepo;

        public OfferService(IUnitOfWork unitOfWork, IGenericRepository<Auction> auctionrepo, IGenericRepository<Offer> offerrepo)
        {
            _unitOfWork = unitOfWork;
            _auctionrepo = auctionrepo;
            _offerrepo = offerrepo;
        }

        public async Task<(OfferDto? offer, string? error)> AddOfferToAuctionAsync(Guid auctionId, CreateOfferDto offerDto, Guid userId)
        {
            var auction = await _auctionrepo.GetByIdAsync(auctionId);

            if (auction == null)
            {
                return (null, "Auction not found.");
            }

            if (auction.CreatorId != userId)
            {
                return (null, "You are not authorized to add offers to this auction.");
            }

            var offer = new Offer
            {
                AuctionId = auctionId,
                Description = offerDto.Description,
                TriggerPrice = offerDto.TriggerPrice
            };

            await _offerrepo.AddAsync(offer);
            await _unitOfWork.CommitAsync();

            return (MapToOfferDto(offer), null);
        }

        public async Task<OfferDto?> GetOfferByIdAsync(Guid offerId)
        {
            var offer = await _offerrepo.GetByIdAsync(offerId);
            return offer == null ? null : MapToOfferDto(offer);
        }

        public async Task<(OfferDto? offer, string? error)> UpdateOfferAsync(Guid offerId, CreateOfferDto offerDto, Guid userId)
        {
            var offer = await _offerrepo.GetByIdAsync(offerId);

            if (offer == null)
            {
                return (null, "Offer not found.");
            }

            // Verify the user is the creator of the auction this offer belongs to.
            var auction = await _auctionrepo.GetByIdAsync(offer.AuctionId);
            if (auction == null)
            {
                // This indicates an orphaned offer, which is a data integrity issue.
                return (null, "Associated auction not found.");
            }

            if (auction.CreatorId != userId)
            {
                return (null, "You are not authorized to update this offer.");
            }

            offer.Description = offerDto.Description ?? offer.Description;
            offer.TriggerPrice = offerDto.TriggerPrice == null ? offer.TriggerPrice : offerDto.TriggerPrice;

            _offerrepo.Update(offer);
            await _unitOfWork.CommitAsync();

            return (MapToOfferDto(offer), null);
        }

        public async Task<(bool success, string? error)> DeleteOfferAsync(Guid offerId, Guid userId)
        {
            // Include Auction to verify the creator's ID
            var offer = await _offerrepo.GetByIdAsync(offerId);

            if (offer == null)
            {
                // Return true for idempotency; the offer is already gone.
                return (true, null);
            }
            var auction = await _auctionrepo.GetByIdAsync(offer.AuctionId);
            if (auction == null)
            {
                // This indicates an orphaned offer, which is a data integrity issue.
                return (false, "Associated auction not found.");
            }
            if (offer.Auction.CreatorId != userId)
            {
                return (false, "You are not authorized to delete this offer.");
            }

            _offerrepo.Remove(offer);
            await _unitOfWork.CommitAsync();

            return (true, null);
        }

        public async Task<IEnumerable<OfferDto>> GetOffersForAuctionAsync(Guid auctionId)
        {
            var offers = await _offerrepo.FindAllByPredicateAsync(o => o.AuctionId == auctionId);
            return offers.Select(MapToOfferDto);
        }

        private static OfferDto MapToOfferDto(Offer offer)
        {
            return new OfferDto
            {
                OfferId = offer.OfferId,
                Description = offer.Description,
                TriggerPrice = offer.TriggerPrice
            };
        }
    }
}
