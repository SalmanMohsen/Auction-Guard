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
    /// Represents a review and rating for a property.
    /// </summary>
    public class Review
    {
        #region Properties
        /// <summary>
        /// Gets or sets the unique identifier for the review.
        /// </summary>
        public Guid ReviewId { get; set; }

        /// <summary>
        /// Gets or sets the rating given (e.g., 1 to 5 stars).
        /// </summary>
        [Column(TypeName = "decimal(2, 1)")]
        public decimal Rating { get; set; }

        /// <summary>
        /// Gets or sets the review comment.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets if the review has been reported.
        /// </summary>
        public bool IsReported { get; set; }

        /// <summary>
        /// Gets or sets the reason for reporting the review.
        /// </summary>
        [StringLength(500)]
        public string ReportReason { get; set; }

        /// <summary>
        /// Gets or sets when the review was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        #endregion

        #region Foreign Keys & Navigation Properties
        /// <summary>
        /// Gets or sets the identifier of the property being reviewed.
        /// </summary>
        public Guid PropertyId { get; set; }

        /// <summary>
        /// Navigation property for the property.
        /// </summary>
        public virtual Property Property { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who wrote this review.
        /// </summary>
        [Required]
        public Guid UserId { get; set; } 
        #endregion

    }
}
