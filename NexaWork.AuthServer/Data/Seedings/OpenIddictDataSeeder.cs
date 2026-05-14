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


    public static async Task SeedSwaggerAPIClientAsync(IServiceProvider services)
    {
        var manager = services.GetRequiredService<IOpenIddictApplicationManager>();

        const string swaggerClientId = "nexawork_client_api_swagger";

        if (await manager.FindByClientIdAsync(swaggerClientId) == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = swaggerClientId,
                ConsentType = ConsentTypes.Implicit,
                DisplayName = "Swagger UI Testing Client",
                RedirectUris = { new Uri("https://localhost:7172/swagger/oauth2-redirect.html") },
                Permissions =
        {
            Permissions.Endpoints.EndSession,
            Permissions.Endpoints.Authorization,
            Permissions.Endpoints.Token,
            Permissions.GrantTypes.AuthorizationCode,
            Permissions.ResponseTypes.Code,
            Permissions.Prefixes.Scope + "api"
        }
            });
        }
    }


    /// <summary>
    /// To prevent hackers from endlessly spamming the Introspection endpoint to guess tokens, 
    /// OpenIddict requires the API itself to authenticate before it will decrypt anything.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static async Task SeedClientAPIIntrospectionAsync(IServiceProvider services)
    {
        var manager = services.GetRequiredService<IOpenIddictApplicationManager>();

        const string apiClientId = "nexawork_client_api";
        const string apiClientSecret = "v_IRV1;OPbz(*OhepHrh!6KYwM1o!!4pVO&MiLFjxJX";


        if (await manager.FindByClientIdAsync(apiClientId) == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = apiClientId,
                ClientSecret = apiClientSecret,
                DisplayName = "NexaWork Client API (Resource Server)",
                Permissions =
        {
            // The API ONLY needs permission to use the Introspection endpoint
            Permissions.Endpoints.Introspection
        }
            });
        }
    }

}
