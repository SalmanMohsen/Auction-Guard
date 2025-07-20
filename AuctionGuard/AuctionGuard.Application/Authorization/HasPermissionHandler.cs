using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionGuard.Application.Authorization
{
    /// <summary>
    /// Handles the <see cref="HasPermissionRequirement"/> to check if a user has the required permission claim.
    /// </summary>
    public class HasPermissionHandler : AuthorizationHandler<HasPermissionRequirement>
    {
        /// <summary>
        /// Makes a decision if authorization is allowed based on a specific requirement.
        /// </summary>
        /// <param name="context">The authorization context.</param>
        /// <param name="requirement">The requirement to evaluate.</param>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionRequirement requirement)
        {
            // Check if the user has a claim of type "Permission" that matches the required permission.
            // The user's claims are populated from the JWT during the authentication process.
            if (context.User.HasClaim(c => c.Type == "Permission" && c.Value == requirement.Permission))
            {
                // If the claim is found, the requirement is met.
                context.Succeed(requirement);
            }

            // If the claim is not found, the handler does nothing, and access is implicitly denied.
            return Task.CompletedTask;
        }
    }
}

