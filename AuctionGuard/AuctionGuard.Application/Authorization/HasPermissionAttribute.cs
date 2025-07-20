using Microsoft.AspNetCore.Authorization;

namespace AuctionGuard.Application.Authorization
{
    /// <summary>
    /// Custom authorization attribute to check for a specific permission.
    /// </summary>
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public const string PolicyPrefix = "HasPermission";

        public HasPermissionAttribute(string permission) => Permission = permission;

        public string Permission
        {
            get => Policy.Substring(PolicyPrefix.Length);
            set => Policy = $"{PolicyPrefix}{value}";
        }
    }
}
