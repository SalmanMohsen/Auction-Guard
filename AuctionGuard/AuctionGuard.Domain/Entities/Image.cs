using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Domain.Entities
{

    /// <summary>
    /// Represents the category or purpose of an image associated with a property.
    /// </summary>
    public enum ImageType 
    { 
        JPEG,
        PNG        
    }

    /// <summary>
    /// Represents an image associated with a property.
    /// </summary>
    public class Image
    {
        #region Properties
        /// <summary>
        /// Gets or sets the unique identifier for the image.
        /// </summary>
        public Guid ImageId { get; set; }

        /// <summary>
        /// Gets or sets if this is the primary image for the property.
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Gets or sets the category of the image.
        /// </summary>
        public ImageType Category { get; set; }

        /// <summary>
        /// Gets or sets information about the image size.
        /// </summary>
        [StringLength(50)]
        public string Size { get; set; }

        /// <summary>
        /// Gets or sets the URL or path to the image file.
        /// </summary>
        [Required]
        [StringLength(2048)]
        public string ImageUrl { get; set; }
        #endregion

        #region Foreign Keys & Navigation Properties
        /// <summary>
        /// Gets or sets the identifier of the property this image belongs to.
        /// </summary>
        public Guid PropertyId { get; set; } // Changed to Guid

        /// <summary>
        /// Navigation property for the property.
        /// </summary>
        public virtual Property Property { get; set; }
        #endregion

    }
}
