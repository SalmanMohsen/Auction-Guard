
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuctionGuard.Domain.Entities
{
    /// <summary>
    /// Represents a user in the system. Inherits from IdentityUser to leverage
    /// ASP.NET Core's robust authentication and user management system.
    /// </summary>
    public class User : IdentityUser<Guid>
    {
        #region Custom Properties
        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        [MaxLength(50)]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's middle name.
        /// </summary>
        [MaxLength(50)]
        public string? MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        [MaxLength(50)]
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the URL for the user's uploaded identification document image.
        /// </summary>
        [Required]
        public string? IdentificationImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the date and time the user registered.
        /// </summary>
        [Required]
        public DateTime RegisterDate { get; set; }
        #endregion

    }
}
