using AuctionGuard.Domain.Entities;
using AuctionGuard.Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Infrastructure.Seeders
{
    /// <summary>
    /// A static class to define all available permissions in the system.
    /// This makes permissions easy to manage and prevents typos.
    /// </summary>
    public static class Permissions
    {
        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";
            public const string Blacklist = "Permissions.Users.Blacklist";
        }

        public static class Properties
        {
            public const string View = "Permissions.Properties.View";
            public const string Create = "Permissions.Properties.Create";
            public const string Edit = "Permissions.Properties.Edit";
            public const string Delete = "Permissions.Properties.Delete";
            public const string Approve = "Permissions.Properties.Approve";
        }

        public static class Auctions
        {
            public const string View = "Permissions.Auctions.View";
            public const string Create = "Permissions.Auctions.Create";
            public const string Edit = "Permissions.Auctions.Edit";
            public const string Delete = "Permissions.Auctions.Delete";
            public const string Participate = "Permissions.Auctions.Participate";
            public const string CreateOffer = "Permissions.Auctions.CreateOffer";
        }

        public static class Bids
        {
            public const string Place = "Permissions.Bids.Place";
            public const string View = "Permissions.Bids.View";
            public const string Withdraw = "Permissions.Bids.Withdraw";
        }

        public static class Reviews
        {
            public const string View = "Permissions.Reviews.View";
            public const string Create = "Permissions.Reviews.Create";
            public const string Manage = "Permissions.Reviews.Manage";
            public const string Report = "Permissions.Reviews.Report";
        }

        public static class Favorites
        {
            public const string Add = "Permissions.Favorites.Add";
            public const string Remove = "Permissions.Favorites.Remove";
            public const string View = "Permissions.Favorites.View";
        }

        public static class Payments
        {
            public const string Process = "Permissions.Payments.Process";
            public const string View = "Permissions.Payments.View";
        }

        public static class Notifications
        {
            public const string ManagePreferences = "Permissions.Notifications.ManagePreferences";
        }
    }


    /// <summary>
    /// Seeds the database with initial Identity data, such as roles, permissions, and admin users.
    /// </summary>
    public static class IdentityDataSeeder
    {
        /// <summary>
        /// Creates default roles and assigns permissions.
        /// </summary>
        /// <param name="serviceProvider">The application's service provider.</param>
        public static async Task SeedRolesAndPermissionsAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AuctionGuardIdentityDbContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

            await SeedPermissionsAsync(context);

            // --- 1. Seed Roles ---
            string[] roleNames = { "Admin", "Seller", "Bidder" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new Role { Name = roleName });
                }
            }

            // --- 2. Define Permissions for Each Role ---
            await context.SaveChangesAsync();

            // 3. Define Permissions for Each Role and Establish Relationships
            await AddPermissionsToRoleAsync(context, "Admin", typeof(Permissions.Users).GetFields().Select(f => f.GetValue(null).ToString()).ToList());
            await AddPermissionsToRoleAsync(context, "Admin", typeof(Permissions.Properties).GetFields().Where(f => f.Name != "Create").Select(f => f.GetValue(null).ToString()).ToList());
            await AddPermissionsToRoleAsync(context, "Admin", typeof(Permissions.Auctions).GetFields().Select(f => f.GetValue(null).ToString()).ToList());
            await AddPermissionsToRoleAsync(context, "Admin", typeof(Permissions.Bids).GetFields().Select(f => f.GetValue(null).ToString()).ToList());
            await AddPermissionsToRoleAsync(context, "Admin", typeof(Permissions.Reviews).GetFields().Select(f => f.GetValue(null).ToString()).ToList());
            await AddPermissionsToRoleAsync(context, "Admin", typeof(Permissions.Favorites).GetFields().Where(f => f.Name != "Add" && f.Name != "Remove").Select(f => f.GetValue(null).ToString()).ToList());
            await AddPermissionsToRoleAsync(context, "Admin", typeof(Permissions.Payments).GetFields().Where(f => f.Name != "Process").Select(f => f.GetValue(null).ToString()).ToList());

            // Seller Permissions
            await AddPermissionsToRoleAsync(context, "Seller", new List<string>
            {
                Permissions.Properties.View,
                Permissions.Properties.Create,
                Permissions.Properties.Edit,
                Permissions.Properties.Delete,
                Permissions.Auctions.View,
                Permissions.Auctions.Create,
                Permissions.Auctions.Edit,
                Permissions.Auctions.Delete,
                Permissions.Auctions.CreateOffer,
                Permissions.Bids.View,
                Permissions.Reviews.View,
                Permissions.Auctions.Participate,
                Permissions.Bids.Place,
                Permissions.Favorites.Add,
                Permissions.Favorites.Remove,
                Permissions.Favorites.View,
                Permissions.Notifications.ManagePreferences,
                Permissions.Reviews.Report,
            });

            // Bidder Permissions
            await AddPermissionsToRoleAsync(context, "Bidder", new List<string>
            {
                Permissions.Properties.View,
                Permissions.Auctions.View,
                Permissions.Auctions.Participate,
                Permissions.Bids.Place,
                Permissions.Bids.View,
                Permissions.Bids.Withdraw,
                Permissions.Reviews.View,
                Permissions.Reviews.Create,
                Permissions.Favorites.Add,
                Permissions.Favorites.Remove,
                Permissions.Favorites.View,
                Permissions.Payments.Process,
                Permissions.Notifications.ManagePreferences,
                Permissions.Reviews.Report,
            });
            await SeedAdminUsersAsync(serviceProvider);
        }

        /// <summary>
        /// Reads all string constants from the static 'Permissions' class and ensures
        /// they exist in the database as Permission entities.
        /// </summary>
        private static async Task SeedPermissionsAsync(AuctionGuardIdentityDbContext context)
        {
            var existingPermissionNames = await context.Permissions.Select(p => p.PermissionName).ToListAsync();

            var allPermissionConstants = typeof(Permissions)
                .GetNestedTypes()
                .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static))
                .Where(f => f.IsLiteral && f.FieldType == typeof(string))
                .Select(f => (string)f.GetValue(null))
                .ToList();

            foreach (var permissionName in allPermissionConstants)
            {
                if (!existingPermissionNames.Contains(permissionName))
                {
                    await context.Permissions.AddAsync(new Permission { PermissionName = permissionName });
                }
            }
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// A helper method to add a list of permissions to a role using the entity relationships.
        /// </summary>
        private static async Task AddPermissionsToRoleAsync(AuctionGuardIdentityDbContext context, string roleName, IEnumerable<string> permissionNames)
        {
            var role = await context.Roles
                                    .Include(r => r.Permissions)
                                    .FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null) return;

            var permissionsToAssign = await context.Permissions
                                                   .Where(p => permissionNames.Contains(p.PermissionName))
                                                   .ToListAsync();

            foreach (var permission in permissionsToAssign)
            {
                if (!role.Permissions.Any(p => p.PermissionId == permission.PermissionId))
                {
                    role.Permissions.Add(permission);
                }
            }
            await context.SaveChangesAsync();
        }


        /// <summary>
        /// Creates default admin users if they don't exist.
        /// </summary>
        private static async Task SeedAdminUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            // --- Admin User 1 ---
            var admin1Email = "admin1@auctionguard.com";
            if (await userManager.FindByEmailAsync(admin1Email) == null)
            {
                var adminUser1 = new User
                {
                    UserName = admin1Email,
                    Email = admin1Email,
                    FirstName = "Admin",
                    LastName = "One",
                    // It's better to let the confirmation step handle this
                    // EmailConfirmed = true, 
                    PhoneNumberConfirmed = true,
                    RegisterDate = DateTime.UtcNow,
                    IdentificationImageUrl = "path/to/default/id.jpg"
                };

                var result = await userManager.CreateAsync(adminUser1, "AdminPass123!");
                if (result.Succeeded)
                {
                    // Explicitly confirm the email after the user is created
                    var user = await userManager.FindByEmailAsync(admin1Email);
                    if (user != null)
                    {
                        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        await userManager.ConfirmEmailAsync(user, token);
                        await userManager.AddToRoleAsync(user, "Admin");
                    }
                }
            }

            // --- Admin User 2 ---
            var admin2Email = "admin2@auctionguard.com";
            if (await userManager.FindByEmailAsync(admin2Email) == null)
            {
                var adminUser2 = new User
                {
                    UserName = "admin2",
                    Email = admin2Email,
                    FirstName = "Admin",
                    LastName = "Two",
                    PhoneNumberConfirmed = true,
                    RegisterDate = DateTime.UtcNow,
                    IdentificationImageUrl = "path/to/default/id.jpg"
                };

                var result = await userManager.CreateAsync(adminUser2, "AdminPass456!");
                if (result.Succeeded)
                {
                    // Explicitly confirm the email after the user is created
                    var user = await userManager.FindByEmailAsync(admin2Email);
                    if (user != null)
                    {
                        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        await userManager.ConfirmEmailAsync(user, token);
                        await userManager.AddToRoleAsync(user, "Admin");
                    }
                }
            }
        }

    }
}
