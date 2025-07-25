using AuctionGuard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.DTOs.PropertyDTOs
{
    /// <summary>
    /// DTO for returning property details to the client.
    /// </summary>
    public class PropertyDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public decimal PriceInitial { get; set; }
        public Guid OwnerId { get; set; }
        public PropertyStatus PropertyStatus { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public PropertyType PropertyType { get; set; }
        public DateOnly ConstructedAt { get; set; }
        public DateOnly? LastRenew { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
