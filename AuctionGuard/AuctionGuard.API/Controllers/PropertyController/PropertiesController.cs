using AuctionGuard.Application.Authorization;
using AuctionGuard.Application.DTOs.PropertyDTOs;
using AuctionGuard.Application.IServices;
using AuctionGuard.Infrastructure.Seeders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AuctionGuard.API.Controllers.PropertyController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertiesController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }


        /// <summary>
        /// Creates a new property. Requires 'Create Properties' permission.
        /// </summary>
        [HttpPost("Create-Property")]
        [HasPermission(Permissions.Properties.Create)]
        public async Task<ActionResult<PropertyDto>> CreateProperty([FromForm] CreatePropertyDto createPropertyDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            var property = await _propertyService.CreatePropertyAsync(createPropertyDto, userId);
            return CreatedAtAction(nameof(GetPropertyById), new { id = property.Id }, property);
        }

        ///<summary>
        /// Gets the properties that the user has if he is a seller.
        /// </summary>
        [HttpGet("GetMyProperties")]
        [HasPermission(Permissions.Properties.Create)]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetSellerProperties()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(id, out Guid sellerId);
           
            var properties = await _propertyService.GetSellerPropertiesAsync(sellerId);
            if (properties == null)
            {
                return NotFound();
            }
            return Ok(properties);
        }

        /// <summary>
        /// Gets a specific property by its ID. This endpoint is open to the public.
        /// </summary>
        [HttpGet("{id:guid}/GetPropertyById")]
        [AllowAnonymous]
        public async Task<ActionResult<PropertyDto>> GetPropertyById(Guid id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            var properties = await _propertyService.GetUnderApprovalPropertiesAsync();
            if (property == null)
            {
                return NotFound();
            }
            if((properties.Contains(property) && User.IsInRole("Bidder")) || (properties.Contains(property) && property.OwnerId.ToString() != User.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                return Forbid();
            }
            return Ok(property);
        }

        /// <summary>
        /// Gets a list of all approved and available properties. This endpoint is open to the public.
        /// </summary>
        [HttpGet("GetApprovedProperties")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetApprovedProperties()
        {
            var properties = await _propertyService.GetApprovedPropertiesAsync();
            return Ok(properties);
        }

        /// <summary>
        /// Gets a list of all under approval properties. This endpoint is open to the admin.
        /// </summary>
        [HttpGet("admin/GetUnderApprovalProperties")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PropertyDto>>> GetUnderApprovalProperties()
        {
            var properties = await _propertyService.GetUnderApprovalPropertiesAsync();
            return Ok(properties);
        }
        /// <summary>
        /// Updates an existing property. Requires 'Edit Properties' permission.
        /// The service layer ensures only the owner can edit.
        /// </summary>
        [HttpPut("{id:guid}/UpdateProperty")]
        [HasPermission(Permissions.Properties.Edit)]
        public async Task<IActionResult> UpdateProperty(Guid id, [FromBody] UpdatePropertyDto updatePropertyDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Invalid user identifier.");
            }
            var isAdmin = User.IsInRole("Admin");
            if(!isAdmin || property.OwnerId != userId)
            {
                return Unauthorized("You are not allowed to edit this property.");
            }
            var updatedProperty = await _propertyService.UpdatePropertyAsync(id, updatePropertyDto);
            if (updatedProperty == null)
            {
                return Forbid();
            }
            return Ok(updatedProperty);
        }

        /// <summary>
        /// Updates the approval status of a property. Requires 'Approve Properties' permission (Admin only).
        /// </summary>
        [HttpPut("admin/{id:guid}/status")]
        [HasPermission(Permissions.Properties.Approve)]
        public async Task<IActionResult> UpdateApprovalStatus(Guid id, [FromBody] UpdateApprovalStatusDto statusDto)
        {
            var result = await _propertyService.UpdatePropertyApprovalStatusAsync(id, statusDto);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }


        /// <summary>
        /// Deletes a property. Requires 'Delete Properties' permission.
        /// The service layer ensures only the owner or an admin can delete.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [HasPermission(Permissions.Properties.Delete)]
        public async Task<IActionResult> DeleteProperty(Guid id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Invalid user identifier.");
            }
            
            var isAdmin = User.IsInRole("Admin");
            if(!isAdmin|| userId != property.OwnerId)
            {
                return Unauthorized("You are not allowed to delete this property.");
            }
            var result = await _propertyService.DeletePropertyAsync(id);

            if (!result )
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}
