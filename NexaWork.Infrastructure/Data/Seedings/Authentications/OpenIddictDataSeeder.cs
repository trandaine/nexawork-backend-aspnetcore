using System;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace NexaWork.Infrastructure.Data.Seedings.Authentications;

public class OpenIddictDataSeeder
{
    public static async Task SeedClientAsync(IServiceProvider services)
    {
        var manager = services.GetRequiredService<IOpenIddictApplicationManager>();

        // Tên định danh cho Frontend React của bạn
        const string clientId = "nexawork_react_spa";

        if (await manager.FindByClientIdAsync(clientId) == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                // Không dùng Client Secret vì SPA (React) chạy trên trình duyệt, không bảo mật được secret
                ConsentType = ConsentTypes.Implicit,
                DisplayName = "NexaWork React Frontend",

                // Cấu hình URL mà OpenIddict sẽ trả JWT về sau khi login thành công
                RedirectUris = { new Uri("http://localhost:5173/callback") },
                PostLogoutRedirectUris = { new Uri("http://localhost:5173/") },

                // Cấp quyền sử dụng Authorization Code Flow với PKCE (Chuẩn bảo mật cao nhất hiện nay)
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.EndSession,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Roles
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
            });
        }
    }
}
