using AuctionGuard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.DTOs.PropertyDTOs
{
    /// <summary>
    /// DTO for updating an existing property. All fields are optional.
    /// </summary>
    public class UpdatePropertyDto
    {
        [StringLength(100, MinimumLength = 5)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Address { get; set; }
        
        public PropertyType? PropertyType { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Initial price must be greater than 0.")]
        public decimal? PriceInitial { get; set; }
    }
}
