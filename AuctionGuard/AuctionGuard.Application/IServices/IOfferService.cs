using AuctionGuard.Application.DTOs.OfferDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.IServices
{
    /// <summary>
    /// Defines the contract for a service that manages offer-related operations.
    /// </summary>
    public interface IOfferService
    {
        /// <summary>
        /// Adds a new offer to an existing auction.
        /// </summary>
        /// <param name="auctionId">The ID of the auction to add the offer to.</param>
        /// <param name="offerDto">The offer data.</param>
        /// <param name="userId">The ID of the user performing the action (must be the auction creator).</param>
        /// <returns>The created offer DTO or an error message.</returns>
        Task<(OfferDto? offer, string? error)> AddOfferToAuctionAsync(Guid auctionId, CreateOfferDto offerDto, Guid userId);

        /// <summary>
        /// Updates an existing offer.
        /// </summary>
        /// <param name="offerId">The ID of the offer to update.</param>
        /// <param name="offerDto">The updated offer data.</param>
        /// <param name="userId">The ID of the user performing the action (must be the auction creator).</param>
        /// <returns>The updated offer DTO or an error message.</returns>
        Task<(OfferDto? offer, string? error)> UpdateOfferAsync(Guid offerId, CreateOfferDto offerDto, Guid userId);

        /// <summary>
        /// Retrieves a single offer by its unique identifier.
        /// </summary>
        /// <param name="offerId">The ID of the offer.</param>
        /// <returns>The offer DTO if found; otherwise, null.</returns>
        Task<OfferDto?> GetOfferByIdAsync(Guid offerId);

        /// <summary>
        /// Deletes an offer from an auction.
        /// </summary>
        /// <param name="offerId">The ID of the offer to delete.</param>
        /// <param name="userId">The ID of the user performing the action (must be the auction creator).</param>
        /// <returns>A boolean indicating success and an optional error message.</returns>
        Task<(bool success, string? error)> DeleteOfferAsync(Guid offerId, Guid userId);

        /// <summary>
        /// Gets all offers for a specific auction.
        /// </summary>
        /// <param name="auctionId">The ID of the auction.</param>
        /// <returns>A list of offer DTOs.</returns>
        Task<IEnumerable<OfferDto>> GetOffersForAuctionAsync(Guid auctionId);
    }
}
