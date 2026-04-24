using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NexaWork.Domain.IdentityEntites;
using Microsoft.EntityFrameworkCore;

namespace NexaWork.Infrastructure.Data.Seedings;

public class IdentityRoleDataSeeder
{
    public static async Task SeedRoleAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<NexaWorkRole>>();
        var hasRoles = await roleManager.Roles.AnyAsync();
        if (!hasRoles)
        {
            var roles = new Dictionary<string, string>
            {
                { "Admin", "Quản trị viên hệ thống với toàn quyền kiểm soát" },
                { "Staff", "Nhân viên quản lý nội dung và hỗ trợ nền tảng" },
                { "Employer", "Nhà tuyển dụng đăng tin và quản lý ứng viên" },
                { "User", "Người dùng tiêu chuẩn (Sinh viên/Ứng viên)" }
            };

            foreach (var role in roles)
            {
                var newRole = new NexaWorkRole(role.Key)
                {
                    Description = role.Value
                };

                await roleManager.CreateAsync(newRole);
            }
        }


    }

}
