using CodeLearn.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CodeLearn.Api.Seed;

public class DataSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DataSeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        await CreateRoleIfNotExists("Admin");
        await CreateRoleIfNotExists("Student");

        await CreateAdminUserIfNotExists();
    }

    private async Task CreateRoleIfNotExists(string roleName)
    {
        var roleExists = await _roleManager.RoleExistsAsync(roleName);

        if (!roleExists)
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    private async Task CreateAdminUserIfNotExists()
    {
        var adminEmail = "admin@codelearn.com";
        var adminPassword = "Admin123!";

        var adminUser = await _userManager.FindByEmailAsync(adminEmail);

        if (adminUser != null)
        {
            return;
        }

        adminUser = new ApplicationUser
        {
            FirstName = "Admin",
            LastName = "CodeLearn",
            Email = adminEmail,
            UserName = adminEmail,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(adminUser, adminPassword);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}