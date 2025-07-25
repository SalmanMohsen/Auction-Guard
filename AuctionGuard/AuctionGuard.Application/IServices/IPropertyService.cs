using AuctionGuard.Application.DTOs.PropertyDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.IServices
{
    /// <summary>
    /// Defines the contract for a service that manages property-related operations.
    /// </summary>
    public interface IPropertyService
    {
        Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createPropertyDto, Guid ownerId);
        Task<PropertyDto?> GetPropertyByIdAsync(Guid id);
        Task<IEnumerable<PropertyDto>> GetApprovedPropertiesAsync();
        Task<IEnumerable<PropertyDto>> GetUnderApprovalPropertiesAsync();
        Task<IEnumerable<PropertyDto>> GetSellerPropertiesAsync(Guid id);
        Task<PropertyDto?> UpdatePropertyAsync(Guid id, UpdatePropertyDto updatePropertyDto, Guid userId);
        Task<bool> UpdatePropertyApprovalStatusAsync(Guid id, UpdateApprovalStatusDto statusDto);
        Task<bool> DeletePropertyAsync(Guid id, Guid userId, bool isAdmin);
        
    }
}
