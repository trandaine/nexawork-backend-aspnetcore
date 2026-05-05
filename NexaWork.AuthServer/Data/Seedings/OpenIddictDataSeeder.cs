using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace NexaWork.AuthServer.Data.Seedings;

public class OpenIddictDataSeeder
{
    public static async Task SeedClientAsync(IServiceProvider services)
    {
        var manager = services.GetRequiredService<IOpenIddictApplicationManager>();


        const string clientId = "nexawork_react_web";

        if (await manager.FindByClientIdAsync(clientId) == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                ConsentType = ConsentTypes.Implicit,
                DisplayName = "NexaWork Web Application",
                PostLogoutRedirectUris = { new Uri("http://localhost:5173/callback/logout") },
                RedirectUris = { new Uri("http://localhost:5173/callback/login") },
                Permissions =
            {
                Permissions.Endpoints.EndSession,
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,


                Permissions.Prefixes.Scope + Scopes.OpenId,
                Permissions.Prefixes.Scope + Scopes.Profile,
                Permissions.Prefixes.Scope + "api"
            }
            });
        }
    }
}
