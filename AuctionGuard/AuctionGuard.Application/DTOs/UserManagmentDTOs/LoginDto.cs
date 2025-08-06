using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.DTOs.UserManagmentDTOs
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }

        public bool RememberMe { get; set; } = false;

        [Required]
        public string Password { get; set; }
    }
}
