using AuctionGuard.Application.Authorization;
using AuctionGuard.Application.DTOs.AuctionDTOs;
using AuctionGuard.Application.DTOs.AuctionParticipationDTOs;
using AuctionGuard.Application.IServices;
using AuctionGuard.Application.Services;
using AuctionGuard.Domain.Entities;
using AuctionGuard.Infrastructure.Seeders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AuctionGuard.API.Controllers.AuctionController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuctionsController : ControllerBase
    {
        private readonly IAuctionService _auctionService;
        private readonly IAuctionParticipationService _participationService;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public AuctionsController(IAuctionService auctionService,
            IAuctionParticipationService participationService,
            IMemoryCache cache,
            IConfiguration configuration)
        {
            _auctionService = auctionService;
            _participationService = participationService;
            _cache = cache;
            _configuration = configuration;
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
        public async Task<IActionResult> GetMyAuctions()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid sellerId))
            {
                return Unauthorized("Invalid user identifier.");
            }
            var auctions = await _auctionService.GetSellerAuctionsAsync(sellerId);
            if(auctions == null)
            {
                return NotFound();
            }
            return Ok(auctions);
        }

        [HttpGet("The-Auctions-I-Won")]
        [HasPermission(Permissions.Auctions.Participate)]
        public async Task<IActionResult> GetMyWonAuctions()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid UserId))
            {
                return Unauthorized("Invalid user identifier.");
            }
            var auctions = await _auctionService.GetMyWonAuctionsAsync(UserId);
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
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAuctionById(Guid Id)
        {
          
            var auction = await _auctionService.GetAuctionByIdAsync(Id.ToString());
            if(auction == null)
            {
                return NotFound();
            }
            return Ok(auction);
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

        #region Participation Endpoints

        [HttpPost("{auctionId:guid}/join")]
        [HasPermission(Permissions.Auctions.Participate)]
        public async Task<IActionResult> JoinAuction(Guid auctionId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            var (response, error) = await _participationService.InitiateJoinProcessAsync(auctionId, userId);
            if (error != null)
            {
                if (response?.ApprovalUrl == "JOINED_NO_DEPOSIT")
                {
                    return Ok(new { message = error, status = "Joined" });
                }
                return BadRequest(new { message = error });
            }

            return Ok(response);
        }

        /// <summary>
        /// Callback endpoint for successful PayPal approval of the deposit hold.
        /// </summary>
        [HttpGet("join/success")]
        [AllowAnonymous] // Must be anonymous
        public async Task<IActionResult> JoinAuctionSuccess([FromQuery] string token) // 'token' is the PayPal orderId
        {
            var frontendBaseUrl = _configuration["ApplicationBaseUrl"];

            // *** PRIMARY FIX IS HERE ***
            // Retrieve the cached object using the token from PayPal.
            if (!_cache.TryGetValue(token, out PayPalOrderCacheDto? cacheEntry) || cacheEntry == null)
            {
                return Redirect($"{frontendBaseUrl}/join-failed?error=invalid_or_expired_order");
            }

            // Extract the IDs from the cached object.
            Guid auctionId = cacheEntry.AuctionId;
            Guid userId = cacheEntry.UserId;

            var (success, error) = await _participationService.ConfirmJoinProcessAsync(token, userId, auctionId);
            if (!success)
            {
                return Redirect($"{frontendBaseUrl}/join-failed?error={Uri.EscapeDataString(error ?? "confirmation_failed")}");
            }

            _cache.Remove(token); // Clean up cache
            return Redirect($"{frontendBaseUrl}/auction/{auctionId}?status=joined_successfully");
        }

        /// <summary>
        /// Callback endpoint for cancelled PayPal approval.
        /// </summary>
        [HttpGet("join/cancel")]
        [AllowAnonymous]
        public IActionResult JoinAuctionCancel()
        {
            var frontendBaseUrl = _configuration["ApplicationBaseUrl"];
            // Redirect user to a page on the frontend explaining the cancellation.
            return Redirect($"{frontendBaseUrl}/auctions?status=join_cancelled");
        }

        // --- Other participation endpoints (Leave, Status) remain the same ---
        [HttpPost("{auctionId:guid}/leave")]
        [HasPermission(Permissions.Auctions.Participate)]
        public async Task<IActionResult> LeaveAuction(Guid auctionId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            var (success, error) = await _participationService.LeaveAuctionAsync(auctionId, userId);
            if (!success)
            {
                return BadRequest(new { message = error });
            }

            return Ok(new { message = error });
        }

        [HttpGet("{auctionId:guid}/participation-status")]
        [HasPermission(Permissions.Auctions.Participate)]
        public async Task<IActionResult> GetParticipationStatus(Guid auctionId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            var status = await _participationService.CheckParticipationStatusAsync(auctionId, userId);
            return Ok(status);
        }


        #endregion
    }
}
