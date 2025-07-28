using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.DTOs.OfferDTO
{
    public class CreateOfferDto
    {
        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal TriggerPrice { get; set; }
    }

}
