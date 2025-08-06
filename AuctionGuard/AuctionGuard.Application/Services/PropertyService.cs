using AuctionGuard.Application.DTOs.PropertyDTOs;
using AuctionGuard.Application.IServices;
using AuctionGuard.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuctionGuard.Domain.Entities;
using Microsoft.AspNetCore.Hosting;

namespace AuctionGuard.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Property> _propertyrepo;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PropertyService(IUnitOfWork unitOfWork, IGenericRepository<Property> propertyrepo, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _propertyrepo = propertyrepo;
            _hostEnvironment = hostEnvironment;
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
                ImageUrls = new List<string>()
            };

            if (createPropertyDto.Images != null)
            {
                // Ensure the directory to save images exists
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");
                Directory.CreateDirectory(uploadsFolder);

                foreach (var imageFile in createPropertyDto.Images)
                {
                    // Create a unique file name to avoid conflicts
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file to the server's file system
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    // Add the public URL to the list
                    property.ImageUrls.Add("/images/" + uniqueFileName);
                }
            }

            await _propertyrepo.AddAsync(property);
            await _unitOfWork.CommitAsync();

            return MapToPropertyDto(property);
        }

        public async Task<PropertyDto?> GetPropertyByIdAsync(Guid id)
        {
            var property = await _propertyrepo.GetByIdAsync(id);
            return property == null ? null : MapToPropertyDto(property);
        }

        public async Task<IEnumerable<PropertyDto>> GetApprovedPropertiesAsync()
        { 
            var properties =  await _propertyrepo.FindAllByPredicateAsync(
                p => p.ApprovalStatus == ApprovalStatus.Approved && p.PropertyStatus == PropertyStatus.Available
            );
            return properties.Select(MapToPropertyDto);
        }

        public async Task<IEnumerable<PropertyDto>> GetSellerPropertiesAsync(Guid id)
        {
            
            var properties = await _propertyrepo.FindAllByPredicateAsync(
                p => p.OwnerId == id
                );
            return properties.Select(MapToPropertyDto);
        }
        public async Task<IEnumerable<PropertyDto>> GetUnderApprovalPropertiesAsync()
        {
            var properties = await _propertyrepo.FindAllByPredicateAsync(
                p => p.ApprovalStatus == ApprovalStatus.UnderApproval
                );
            return properties.Select(MapToPropertyDto);
        }

        public async Task<PropertyDto?> UpdatePropertyAsync(Guid id, UpdatePropertyDto updatePropertyDto)
        {
            var property = await _propertyrepo.GetByIdAsync(id);
            
            if (property == null)
            {
                return null;
            }
            
            property.Title = updatePropertyDto.Title ?? property.Title;
            property.Description = updatePropertyDto.Description ?? property.Description;
            property.Address = updatePropertyDto.Address ?? property.Address;
            property.PriceInitial = updatePropertyDto.PriceInitial ?? property.PriceInitial;

            _propertyrepo.Update(property);
            await _unitOfWork.CommitAsync();

            return MapToPropertyDto(property);
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

        public async Task<bool> DeletePropertyAsync(Guid id)
        {
            var property = await _propertyrepo.GetByIdAsync(id);

            if (property == null)
            {
                return false;
            }
            foreach (var imageUrl in property.ImageUrls)
            {
                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }
            }
            _propertyrepo.Remove(property);
            await _unitOfWork.CommitAsync();
            return true;
        }

        private PropertyDto MapToPropertyDto(Property property)
        {
            return new PropertyDto
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
                ImageUrls = property.ImageUrls ?? new List<string>()
            };
        }
    }
}
