using AuctionGuard.Application.Authorization;
using AuctionGuard.Application.DTOs.OfferDTO;
using AuctionGuard.Application.IServices;
using AuctionGuard.Infrastructure.Seeders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AuctionGuard.API.Controllers.OffersController
{
    [Route("api/offers")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OffersController : ControllerBase
    {
        private readonly IOfferService _offerService;

        public OffersController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        [HttpPost("auctions/{auctionId:guid}")]
        [HasPermission(Permissions.Auctions.Edit)] 
        public async Task<IActionResult> AddOfferToAuction(Guid auctionId, [FromBody] CreateOfferDto offerDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            var (offer, error) = await _offerService.AddOfferToAuctionAsync(auctionId, offerDto, userId);

            if (error != null)
            {
                return BadRequest(new { message = error });
            }

            return CreatedAtAction(nameof(GetOffer), new { offerId = offer.OfferId }, offer);
        }

        [HttpPut("{offerId:guid}")]
        [HasPermission(Permissions.Auctions.Edit)]
        public async Task<IActionResult> UpdateOffer(Guid offerId, [FromBody] CreateOfferDto offerDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            var (offer, error) = await _offerService.UpdateOfferAsync(offerId, offerDto, userId);

            if (error != null)
            {
                return BadRequest(new { message = error });
            }

            return Ok(offer);
        }

        [HttpDelete("{offerId:guid}")]
        [HasPermission(Permissions.Auctions.Delete)] // Or Auctions.Edit
        public async Task<IActionResult> DeleteOffer(Guid offerId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            var (success, error) = await _offerService.DeleteOfferAsync(offerId, userId);

            if (!success)
            {
                return BadRequest(new { message = error });
            }

            return NoContent();
        }

        // Helper endpoint to retrieve a single offer, used by CreatedAtAction
        [HttpGet("{offerId:guid}", Name = "GetOffer")]
        [HasPermission(Permissions.Auctions.View)]
        public async Task<IActionResult> GetOffer(Guid offerId)
        {
            // This logic could be expanded in IOfferService if needed
            var offer = await _offerService.GetOfferByIdAsync(offerId);
            if (offer == null)
            {
                return NotFound();
            }
            return Ok(offer);
        }

        [HttpGet("auctions/{auctionId:guid}")]
        [HasPermission(Permissions.Auctions.View)]
        public async Task<IActionResult> GetOffersForAuction(Guid auctionId)
        {
            var offers = await _offerService.GetOffersForAuctionAsync(auctionId);
            return Ok(offers);
        }
    }
}
