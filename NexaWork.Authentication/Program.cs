using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NexaWork.Authentication.Data;
using NexaWork.Authentication.Data.IdentityEntities;
using NexaWork.Authentication.Data.Seedings;
using OpenIddict.Abstractions;
using Quartz;
using MassTransit;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<NexaWorkIdentityDbContext>(options =>
{
    options.UseOpenIddict();
});


builder.Services.AddIdentity<NexaWorkUser, NexaWorkRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Cho phép đăng nhập mà không cần xác nhận email
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    // User settings
    options.User.RequireUniqueEmail = true;
    // Sign-in settings
    options.SignIn.RequireConfirmedEmail = false;  // Sign in không cần confirm email
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
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


// builder.Services.AddApplicationServices();
// builder.Services.AddInfrastructureServices(builder.Configuration);


// Configure MassTransit with RabbitMQ (The Publisher)
var rabbitMqSettings = builder.Configuration
    .GetSection("RabbitMQ");

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(
            rabbitMqSettings["Host"],
            rabbitMqSettings["VirtualHost"],
            h =>
            {
                // NOTE: Do NOT store production secrets directly in appsettings.json.
                h.Username(rabbitMqSettings["Username"]!);
                h.Password(rabbitMqSettings["Password"]!);
            });
    });
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
               .SetTokenEndpointUris("connect/token")
               .SetIntrospectionEndpointUris("connect/introspect");

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


        // Disable access token encryption (makes it easier to debug with tools like jwt.ms or jwt.io)
        // OpenIddict to issue standard, readable JWTs instead of encrypted opaque tokens
        // options.DisableAccessTokenEncryption();


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
    options.AddPolicy("AllowAppsAccess", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173", // React app
            "https://localhost:7172" // Client API
            )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



// Add services to the container.

builder.Services.AddControllersWithViews();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("AllowAppsAccess");

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
        await OpenIddictDataSeeder.SeedSwaggerAPIClientAsync(services);
        await OpenIddictDataSeeder.SeedClientAPIIntrospectionAsync(services);
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

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    // pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
