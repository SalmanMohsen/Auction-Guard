using Microsoft.AspNetCore.Authorization;

namespace AuctionGuard.Application.Authorization
{
    /// <summary>
    /// Represents a requirement for a user to have a specific permission.
    /// This class is used by the authorization policy to specify which permission is needed.
    /// </summary>
    public class HasPermissionRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Gets the name of the permission required for authorization.
        /// </summary>
        public string Permission { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HasPermissionRequirement"/> class.
        /// </summary>
        /// <param name="permission">The name of the permission that is required.</param>
        public HasPermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
