using AuctionGuard.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuctionGuard.Application.DTOs.PropertyDTOs
{
    /// <summary>
    /// DTO for creating a new property.
    /// </summary>
    public class CreatePropertyDto
    {
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Initial price must be greater than 0.")]
        public decimal PriceInitial { get; set; }

        [Required]
        public DateOnly ConstructedOn { get; set; }

        public DateOnly LastRenew {  get; set; }

        [Required]
        
        public PropertyType PropertyType { get; set; }

        [MaxLength(25, ErrorMessage = "You can upload a maximum of 25 images.")]
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}