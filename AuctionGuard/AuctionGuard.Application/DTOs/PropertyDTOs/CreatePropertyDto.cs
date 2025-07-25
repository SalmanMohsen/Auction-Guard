using AuctionGuard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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


        /// <summary>
        /// A list of URLs for the property images.
        /// Image upload should be handled separately to get the URLs.
        /// </summary>
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}