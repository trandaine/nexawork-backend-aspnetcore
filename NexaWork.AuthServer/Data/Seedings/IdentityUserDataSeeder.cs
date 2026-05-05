using Microsoft.AspNetCore.Identity;
using NexaWork.AuthServer.Data.IdentityEntities;

namespace NexaWork.AuthServer.Data.Seedings;

public class IdentityUserDataSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<NexaWorkUser>>();

        var adminEmail = "admin@nexawork.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdmin = new NexaWorkUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
            };

            var createPowerUser = await userManager.CreateAsync(newAdmin, "Admin@123456");

            if (createPowerUser.Succeeded)
            {
                // Gán quyền Admin cho tài khoản này
                await userManager.AddToRoleAsync(newAdmin, "Admin");
            }
        }

    }
}
