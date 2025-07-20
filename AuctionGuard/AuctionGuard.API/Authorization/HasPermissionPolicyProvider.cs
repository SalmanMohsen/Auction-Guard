using AuctionGuard.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace AuctionGuard.API.Authorization
{
    /// <summary>
    /// A custom authorization policy provider that creates policies dynamically
    /// for the HasPermissionAttribute.
    /// </summary>
    public class HasPermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _backupPolicyProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="HasPermissionPolicyProvider"/> class.
        /// </summary>
        /// <param name="options">The authorization options.</param>
        public HasPermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            // The DefaultAuthorizationPolicyProvider can handle policies that are registered
            // manually, such as those using services.AddAuthorization(options => ...).
            _backupPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        /// <inheritdoc />
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _backupPolicyProvider.GetDefaultPolicyAsync();

        /// <inheritdoc />
        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => _backupPolicyProvider.GetFallbackPolicyAsync();

        /// <summary>
        /// Gets an authorization policy for a given policy name.
        /// </summary>
        /// <param name="policyName">The name of the policy to retrieve.</param>
        /// <returns>
        /// A Task that represents the asynchronous operation. The task result contains the
        /// AuthorizationPolicy from the authorization provider, or null if the policy is not found.
        /// </returns>
        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // Check if the policy name starts with our custom prefix.
            if (policyName.StartsWith(HasPermissionAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
            {
                // Extract the permission from the policy name (e.g., "Permissions.Users.View").
                var permission = policyName.Substring(HasPermissionAttribute.PolicyPrefix.Length);

                // Create a new policy builder.
                var policy = new AuthorizationPolicyBuilder();

                // Add our custom requirement to the policy, passing the required permission.
                policy.AddRequirements(new HasPermissionRequirement(permission));

                // Build and return the policy.
                return Task.FromResult<AuthorizationPolicy?>(policy.Build());
            }

            // If the policy name doesn't match our prefix, fall back to the default provider.
            // This ensures that other policies defined elsewhere in the application still work.
            return _backupPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}
