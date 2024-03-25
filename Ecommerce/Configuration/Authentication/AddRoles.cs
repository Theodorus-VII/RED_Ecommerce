using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Configuration;

public static class ConfigureRoles
{
    public static async Task AddRoles(this IServiceProvider serviceProvider, string[] roles)
    {
        roles ??= new string[] { "Admin", "User" };

        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }
}
