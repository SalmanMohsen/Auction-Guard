using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Http;

namespace AuctionGuard.Domain.Entities
{
    /// <summary>
    /// Represents the current status of a property listing.
    /// </summary>
    public enum PropertyStatus
    {
        Available, 
        UnderAuction, 
        Sold
       
    }

    /// <summary>
    /// Defines the type of a property.
    /// </summary>
    public enum PropertyType
    { 
        Apartment, 
        Villa, 
        Land, 
        Commercial, 
        Other 
    }

    /// <summary>
    /// Represents the approval status of a property listing by an administrator.
    /// </summary>
    public enum ApprovalStatus
    {  
        Approved, 
        UnderApproval,
        Rejected 
    }

    /// <summary>
    /// Represents a property or item being listed for auction.
    /// </summary>
    public class Property
    {
        #region Properties
        /// <summary>
        /// Gets or sets the primary key for the property.
        /// </summary>
        public Guid PropertyId { get; set; }

        /// <summary>
        /// Gets or sets the title or name of the property.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the detailed description of the property.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the physical address of the property.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the geographical coordinates (e.g., latitude, longitude).
        /// </summary>
        public string? LocationCoordinates { get; set; }

        /// <summary>
        /// Gets or sets the initial or estimated price of the property.
        /// </summary>
        public decimal PriceInitial { get; set; }

        /// <summary>
        /// Gets or sets the final price the property was sold for.
        /// </summary>
        public decimal? EndedPrice { get; set; }

        ///<summary>
        ///Gets or sets the date of the property construction
        ///</summary>
        public DateOnly ConstructedOn { get; set; }

        /// <summary>
        /// Gets or sets the current status of the property (e.g. 'Available', 'Sold', 'Inactive').
        /// </summary>
        public PropertyStatus PropertyStatus { get; set; }

        /// <summary>
        /// Gets or sets the date the listing was last renewed.
        /// </summary>
        public DateOnly? LastRenew { get; set; }

        /// <summary>
        /// Gets or sets the category or type of the property.
        /// </summary>
        public PropertyType PropertyType { get; set; }

        /// <summary>
        /// Gets or sets the approval status by moderators (e.g., 'Pending', 'Approved', 'Rejected').
        /// </summary>
        public ApprovalStatus ApprovalStatus { get; set; }
        #endregion

        #region Foreign Keys & Navigation Properties
        /// <summary>
        /// Gets or sets the foreign key for the User who owns this property.
        /// </summary>
        public Guid OwnerId { get; set; }
        #endregion

        #region Navigation Collections
        /// <summary>
        /// Gets the collection of auctions this property has been listed in.
        /// </summary>
        public ICollection<Auction> Auctions { get; set; } = new HashSet<Auction>();

        
        /// <summary>
        /// Gets the collection of images associated with this property.
        /// </summary>
        [NotMapped]
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();


        public List<string> ImageUrls { get; set; } = new List<string>();
        /// <summary>
        /// Gets the collection of tags associated with this property.
        /// </summary>
        public ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();

        /// <summary>
        /// Gets the collection of join entities representing the users who have favorited this property.
        /// </summary>
        public ICollection<FavoriteProperty> FavoritedByUsers { get; set; } = new HashSet<FavoriteProperty>();

        /// <summary>
        /// Gets the collection of reviews for this property.
        /// </summary>
        public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
        #endregion
    }
}
