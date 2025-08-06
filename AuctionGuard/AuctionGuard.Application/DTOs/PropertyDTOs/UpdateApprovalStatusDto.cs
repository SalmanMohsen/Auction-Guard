using AuctionGuard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuctionGuard.Application.DTOs.PropertyDTOs
{
    /// <summary>
    /// DTO specifically for updating a property's approval status by an admin.
    /// </summary>
    public class UpdateApprovalStatusDto
    {
        [Required]
        [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Approval status must be either 'Approved' or 'Rejected'.")]
        
        public ApprovalStatus ApprovalStatus { get; set; }
    }
}
