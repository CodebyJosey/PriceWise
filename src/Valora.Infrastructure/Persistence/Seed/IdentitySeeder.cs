using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Valora.Infrastructure.Persistence.Identity;

namespace Valora.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeds identity roles and an initial administrator account.
/// </summary>
public static class IdentitySeeder
{
    /// <summary>
    /// Seeds roles and the initial administrator user.
    /// </summary>
    /// <param name="services">The root service provider.</param>
    public static async Task SeedAsync(IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();

        RoleManager<IdentityRole<Guid>> roleManager =
            scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        UserManager<ApplicationUser> userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        IConfiguration configuration =
            scope.ServiceProvider.GetRequiredService<IConfiguration>();

        await SeedRolesAsync(roleManager);
        await SeedAdminAsync(userManager, configuration);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        foreach (string roleName in ApplicationRoles.All)
        {
            bool exists = await roleManager.RoleExistsAsync(roleName);
            if (exists)
            {
                continue;
            }

            IdentityResult result = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create role '{roleName}': {string.Join("; ", result.Errors.Select(x => x.Description))}"
                );
            }
        }
    }

    private static async Task SeedAdminAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        string? email = configuration["Seed:Admin:Email"];
        string? password = configuration["Seed:Admin:Password"];
        string? displayName = configuration["Seed:Admin:DisplayName"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        ApplicationUser? existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser is null)
        {
            ApplicationUser adminUser = new ApplicationUser()
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                DisplayName = string.IsNullOrWhiteSpace(displayName) ? "Administrator" : displayName,
                EmailConfirmed = true,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            IdentityResult createResult = await userManager.CreateAsync(adminUser, password);

            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to create seeded admin user '{email}': {string.Join("; ", createResult.Errors.Select(x => x.Description))}");
            }

            existingUser = adminUser;
        }

        bool isInRole = await userManager.IsInRoleAsync(existingUser, ApplicationRoles.Admin);

        if(!isInRole)
        {
            IdentityResult roleResult = await userManager.AddToRoleAsync(existingUser, ApplicationRoles.Admin);

            if(!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to assign Admin role to seeded user '{email}': {string.Join("; ", roleResult.Errors.Select(x => x.Description))}");
            }
        }
    }
}