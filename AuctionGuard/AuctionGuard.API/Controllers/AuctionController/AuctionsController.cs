using AuctionGuard.Application.Authorization;
using AuctionGuard.Application.DTOs.AuctionDTOs;
using AuctionGuard.Application.IServices;
using AuctionGuard.Application.Services;
using AuctionGuard.Infrastructure.Seeders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuctionGuard.API.Controllers.AuctionController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuctionsController : ControllerBase
    {
        private readonly IAuctionService _auctionService;

        public AuctionsController(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        [HttpPost]
        [HasPermission(Permissions.Auctions.Create)]
        public async Task<IActionResult> CreateAuction([FromBody] CreateAuctionDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid sellerId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            var (auction, error) = await _auctionService.CreateAuctionAsync(dto, sellerId);

            if (error != null)
            {
                return BadRequest(new { message = error });
            }

            return Ok(auction);
        }

        

        [HttpGet("My-Auctions")]
        [HasPermission(Permissions.Auctions.Create)]
        public async Task<IActionResult> GetMyAuctions(Guid id)
        {
            var auctions = await _auctionService.GetSellerAuctionsAsync(id);
            if(auctions == null)
            {
                return NotFound();
            }
            return Ok(auctions);
        }

        [HttpGet("The-Auctions-I-Won")]
        [HasPermission(Permissions.Auctions.Participate)]
        public async Task<IActionResult> GetMyWonAuctions(Guid id)
        {
            var auctions = await _auctionService.GetMyWonAuctionsAsync(id);
            if(auctions == null)
            {
                return NotFound();
            }
            return Ok(auctions);
        }


        [HttpPut("{auctionId:guid}")]
        [HasPermission(Permissions.Auctions.Edit)]
        public async Task<IActionResult> UpdateAuction(Guid auctionId, [FromBody] UpdateAuctionDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            var (auction, error) = await _auctionService.UpdateAuctionAsync(auctionId, dto, userId);

            if (error != null)
            {
                return BadRequest(new { message = error });
            }

            return Ok(auction);
        }


        [HttpDelete("{auctionId:guid}")]
        [HasPermission(Permissions.Auctions.Delete)]
        public async Task<IActionResult> DeleteAuction(Guid auctionId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Invalid user identifier.");
            }
            var isAdmin = User.IsInRole("Admin");
            var (success, error) = await _auctionService.DeleteAuctionAsync(auctionId, userId, isAdmin);

            if (!success)
            {
                return BadRequest(new { message = error });
            }

            return NoContent();
        }

        [HttpGet("ended")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEndedAuctions()
        {
            var auctions = await _auctionService.GetEndedAuctionsAsync();
            return Ok(auctions);
        }

        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveAuctions()
        {
            var auctions = await _auctionService.GetActiveAuctionsAsync();
            return Ok(auctions);
        }

        [HttpPatch("{auctionId:guid}/cancel")]
        [HasPermission(Permissions.Auctions.Edit)] // Assuming cancel is a form of editing
        public async Task<IActionResult> CancelAuction(Guid auctionId, [FromBody] CancelAuctionDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Invalid user identifier.");
            }
            var isAdmin = User.IsInRole("Admin");

            var (success, error) = await _auctionService.CancelAuctionAsync(auctionId, userId, isAdmin, dto.Reason);

            if (!success)
            {
                // Provide a more specific error for authorization failure
                if (error == "You are not authorized to cancel this auction.")
                {
                    return Forbid();
                }
                return BadRequest(new { message = error });
            }

            return NoContent();
        }

        [HttpGet("scheduled")]
        [AllowAnonymous]
        public async Task<IActionResult> GetScheduledAuctions()
        {
            var auctions = await _auctionService.GetScheduledAuctionsAsync();
            return Ok(auctions);
        }

        [HttpGet("cancelled")]
        [HasPermission(Permissions.Auctions.Create)]
        public async Task<IActionResult> GetCancelledAuctions()
        {
            var auctions = await _auctionService.GetCancelledAuctionsAsync();
            return Ok(auctions);
        }

        [HttpPost("{auctionId:guid}/remake")]
        [HasPermission(Permissions.Auctions.Create)] // Re-creating is a form of creation
        public async Task<IActionResult> RemakeAuction(Guid auctionId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid sellerId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            var (success, error) = await _auctionService.AttemptRemakeAuctionAsync(auctionId, sellerId);

            if (!success)
            {
                return BadRequest(new { message = error });
            }

            return Ok(new { message = error }); // Success message from service
        }
    }
}
