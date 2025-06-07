using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Domain.Entities
{
    /// <summary>
    /// Represents a tag for properties.
    /// </summary>
    public class Tag
    {
        #region Properties
        /// <summary>
        /// Gets or sets the unique identifier for the tag.
        /// </summary>
        public Guid TagId { get; set; } 

        /// <summary>
        /// Gets or sets the name of the tag.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string TagName { get; set; }
        #endregion

        #region Navigation Collections
        /// <summary>
        /// Gets the collection of properties associated with this tag.
        /// </summary>
        public ICollection<Property> Properties { get; private set; } = new HashSet<Property>();
        #endregion
    }
}
