using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NexaWork.AuthServer.Data;
using NexaWork.AuthServer.Data.IdentityEntities;
using NexaWork.AuthServer.Data.Seedings;
using OpenIddict.Abstractions;
using Quartz;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<NexaWorkIdentityDbContext>(options =>
{
    options.UseOpenIddict();
});


builder.Services.AddIdentity<NexaWorkUser, NexaWorkRole>()
    .AddEntityFrameworkStores<NexaWorkIdentityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});

// Add Quartz services
builder.Services.AddQuartz(options =>
{
    options.UseSimpleTypeLoader();
    options.UseInMemoryStore();
});

// Register the Quartz backgound service
builder.Services.AddQuartzHostedService(options =>
{
    // Wait for active jobs to finish before shutting down the server
    options.WaitForJobsToComplete = true;
});




builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<NexaWorkIdentityDbContext>();

        options.UseQuartz()
        // By default, OpenIddict waits until tokens are 14 days old before deleting them.
        // If you want it to aggressively delete younger tokens, you can lower the lifespan limit
        // (Note: The absolute minimum lifespan OpenIddict allows is 10 minutes)
        // Note: This is not required when cleanup every 15 minutes. This is make the database overwhelmed with Delete command. Consider turn off when Production.
            .SetMinimumTokenLifespan(TimeSpan.FromMinutes(15))
            .SetMinimumAuthorizationLifespan(TimeSpan.FromMinutes(15));

    })
    .AddServer(options =>
    {

        options.SetEndSessionEndpointUris("connect/logout");

        // Define your OAuth2 endpoints
        options.SetAuthorizationEndpointUris("connect/authorize")
               .SetTokenEndpointUris("connect/token");

        // Enable the Authorization Code Flow with PKCE (Crucial for React/React Native)
        options.AllowAuthorizationCodeFlow()
               .RequireProofKeyForCodeExchange()
               .AllowRefreshTokenFlow();

        // Register scopes (what the client is allowed to access)
        // options.RegisterScopes("api");
        options.RegisterScopes(

            OpenIddictConstants.Scopes.OpenId,
            OpenIddictConstants.Scopes.Profile,
            "api"
        );

        // Development keys (do not use in production)
        options.AddDevelopmentEncryptionCertificate()
               // Add signing certificate for JWT tokens (also for development)
               .AddDevelopmentSigningCertificate();

        // Integrate with ASP.NET Core
        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough()
               .EnableAuthorizationEndpointPassthrough()
               .EnableEndSessionEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Your exact React URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



// Add services to the container.

builder.Services.AddControllersWithViews();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("AllowReactApp");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<NexaWorkIdentityDbContext>();
    // Tự động apply migration và tạo DB nếu chưa có
    // context.Database.Migrate();

    try
    {
        await IdentityRoleDataSeeder.SeedRoleAsync(services);
        await IdentityUserDataSeeder.SeedAdminAsync(services);
        await OpenIddictDataSeeder.SeedClientAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi khi Seeding dữ liệu.");
    }
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
