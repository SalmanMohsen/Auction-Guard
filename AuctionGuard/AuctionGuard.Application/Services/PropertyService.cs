using AuctionGuard.Application.DTOs.PropertyDTOs;
using AuctionGuard.Application.IServices;
using AuctionGuard.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionGuard.Domain.Entities;

namespace AuctionGuard.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Property> _propertyrepo;

        public PropertyService(IUnitOfWork unitOfWork, IGenericRepository<Property> propertyrepo)
        {
            _unitOfWork = unitOfWork;
            _propertyrepo = propertyrepo;
        }

        public async Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createPropertyDto, Guid ownerId)
        {
            var property = new Property
            {
                Title = createPropertyDto.Title,
                Description = createPropertyDto.Description,
                Address = createPropertyDto.Address,
                PriceInitial = createPropertyDto.PriceInitial,
                OwnerId = ownerId,
                PropertyType = createPropertyDto.PropertyType,
                PropertyStatus = PropertyStatus.Available,
                ApprovalStatus = ApprovalStatus.UnderApproval,
                ConstructedOn = createPropertyDto.ConstructedOn,
                Images = createPropertyDto.ImageUrls.Select(url => new Image { ImageUrl = url }).ToList()
            };

            await _propertyrepo.AddAsync(property);
            await _unitOfWork.CommitAsync();


            return new PropertyDto
            {
                Id = property.PropertyId,
                Title = property.Title,
                Description = property.Description,
                Address = property.Address,
                PriceInitial = property.PriceInitial,
                OwnerId = property.OwnerId,
                PropertyType = property.PropertyType,
                PropertyStatus = PropertyStatus.Available,
                ApprovalStatus = ApprovalStatus.UnderApproval,
                ConstructedAt = property.ConstructedOn,
                LastRenew = property.LastRenew,
                ImageUrls = property.Images.Select(i => i.ImageUrl).ToList()

            };
        }

        public async Task<PropertyDto?> GetPropertyByIdAsync(Guid id)
        {
            var property = await _propertyrepo.GetByIdAsync(id);
            return property == null ? null : new PropertyDto
            {
                Id = property.PropertyId,
                Title = property.Title,
                Description = property.Description,
                Address = property.Address,
                PriceInitial = property.PriceInitial,
                OwnerId = property.OwnerId,
                PropertyType = property.PropertyType,
                PropertyStatus = property.PropertyStatus,
                ApprovalStatus = property.ApprovalStatus,
                ConstructedAt = property.ConstructedOn,
                LastRenew = property.LastRenew,
                ImageUrls = property.Images.Select(i => i.ImageUrl).ToList()
            };
        }

        public async Task<IEnumerable<PropertyDto>> GetApprovedPropertiesAsync()
        { 
            var properties =  await _propertyrepo.FindAllByPredicateAsync(
                p => p.ApprovalStatus == ApprovalStatus.Approved && p.PropertyStatus == PropertyStatus.Available
            );
            //return properties.Select(MapToPropertyDto);
            var result = new List<PropertyDto>();
            foreach (var property in properties)
            {
                result.Add(new PropertyDto
                {
                    Id = property.PropertyId,
                    Title = property.Title,
                    Description = property.Description,
                    Address = property.Address,
                    PriceInitial =property.PriceInitial,
                    OwnerId = property.OwnerId,
                    PropertyStatus = property.PropertyStatus,
                    ApprovalStatus = property.ApprovalStatus,
                    ConstructedAt = property.ConstructedOn,
                    LastRenew = property.LastRenew,
                    ImageUrls = property.Images.Select(i => i.ImageUrl).ToList()
                });
            }
            return result;
        }

        public async Task<IEnumerable<PropertyDto>> GetSellerPropertiesAsync(Guid id)
        {
            var properties = await _propertyrepo.FindAllByPredicateAsync(
                p => p.OwnerId == id
                );
            var result = new List<PropertyDto>();
            foreach (var property in properties)
            {
                result.Add(new PropertyDto
                {
                    Id = property.PropertyId,
                    Title = property.Title,
                    Description = property.Description,
                    Address = property.Address,
                    PriceInitial = property.PriceInitial,
                    OwnerId = property.OwnerId,
                    PropertyStatus = property.PropertyStatus,
                    ApprovalStatus = property.ApprovalStatus,
                    ConstructedAt = property.ConstructedOn,
                    LastRenew = property.LastRenew,
                    ImageUrls = property.Images.Select(i => i.ImageUrl).ToList()
                });
            }
            return result;
        }
        public async Task<IEnumerable<PropertyDto>> GetUnderApprovalPropertiesAsync()
        {
            var properties = await _propertyrepo.FindAllByPredicateAsync(
                p => p.ApprovalStatus == ApprovalStatus.UnderApproval
                );
            var result = new List<PropertyDto>();
            foreach (var property in properties)
            {
                result.Add(new PropertyDto
                {
                    Id = property.PropertyId,
                    Title = property.Title,
                    Description = property.Description,
                    Address = property.Address,
                    PriceInitial = property.PriceInitial,
                    OwnerId = property.OwnerId,
                    PropertyStatus = property.PropertyStatus,
                    ApprovalStatus = property.ApprovalStatus,
                    ConstructedAt = property.ConstructedOn,
                    LastRenew = property.LastRenew,
                    ImageUrls = property.Images.Select(i => i.ImageUrl).ToList()
                });
            }
            return result;
        }

        public async Task<PropertyDto?> UpdatePropertyAsync(Guid id, UpdatePropertyDto updatePropertyDto, Guid userId)
        {
            var property = await _propertyrepo.GetByIdAsync(id);
            
            if (property == null || property.OwnerId != userId)
            {
                return null;
            }
            
            property.Title = updatePropertyDto.Title ?? property.Title;
            property.Description = updatePropertyDto.Description ?? property.Description;
            property.Address = updatePropertyDto.Address ?? property.Address;
            property.PriceInitial = updatePropertyDto.PriceInitial ?? property.PriceInitial;

            _propertyrepo.Update(property);
            await _unitOfWork.CommitAsync();

            return new PropertyDto
            {
                Id = property.PropertyId,
                Title = property.Title,
                Description = property.Description,
                Address = property.Address,
                PriceInitial = property.PriceInitial,
                OwnerId = property.OwnerId,
                PropertyStatus = property.PropertyStatus,
                ApprovalStatus = property.ApprovalStatus,
                ConstructedAt = property.ConstructedOn,
                LastRenew = property.LastRenew,
                ImageUrls = property.Images.Select(i => i.ImageUrl).ToList()
            };
        }

        public async Task<bool> UpdatePropertyApprovalStatusAsync(Guid id, UpdateApprovalStatusDto statusDto)
        {
            var property = await _propertyrepo.GetByIdAsync(id);
            if (property == null)
            {
                return false;
            }

            property.ApprovalStatus = statusDto.ApprovalStatus;

            _propertyrepo.Update(property);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<bool> DeletePropertyAsync(Guid id, Guid userId, bool isAdmin)
        {
            var property = await _propertyrepo.GetByIdAsync(id);

            if (property == null)
            {
                return false;
            }

            if (!isAdmin && property.OwnerId != userId)
            {
                return false;
            }

            _propertyrepo.Remove(property);
            await _unitOfWork.CommitAsync();
            return true;
        }

        
    }
}
