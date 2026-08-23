using Microsoft.AspNetCore.Identity;

namespace GroceryShopping.Data;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(
        RoleManager<IdentityRole> roleManager)
    {
        string[] roles =
        {
            "Admin",
            "Customer"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new IdentityRole(role)
                );
            }
        }
    }

    public static async Task SeedAdminAsync(
        UserManager<IdentityUser> userManager,
        IConfiguration configuration)
    {
        var email =
            configuration["AdminAccount:Email"];

        var password =
            configuration["AdminAccount:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Admin account configuration is missing."
            );
        }

        var admin =
            await userManager.FindByEmailAsync(email);

        if (admin == null)
        {
            admin = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result =
                await userManager.CreateAsync(
                    admin,
                    password
                );

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description)
                );

                throw new InvalidOperationException(
                    $"Failed to create admin: {errors}"
                );
            }
        }

        if (!await userManager.IsInRoleAsync(
                admin,
                "Admin"))
        {
            var roleResult =
                await userManager.AddToRoleAsync(
                    admin,
                    "Admin"
                );

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(e => e.Description)
                );

                throw new InvalidOperationException(
                    $"Failed to assign Admin role: {errors}"
                );
            }
        }
    }
}