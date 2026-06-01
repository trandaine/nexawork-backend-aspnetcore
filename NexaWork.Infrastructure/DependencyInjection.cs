using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Domain.IdentityEntites;
using NexaWork.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using NexaWork.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using NexaWork.Application.Common.Interfaces.Services;
using NexaWork.Infrastructure.Services;

namespace NexaWork.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
    IConfiguration configuration
    )
    {
        // 1. Register the DbContext (Your Database Connection)
        // services.AddDbContext<NexaWorkDbContext>(options =>
        //     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
        //         builder => builder.MigrationsAssembly(typeof(NexaWorkDbContext).Assembly.FullName)));
        services.AddDbContext<NexaWorkDbContext>();
        // services.AddDbContext<NexaWorkDbIdentityContext>();


        // Note: Temporary commented out this section to avoid conflict with Auth Server. 
        #region Identity Configuration

        // services.AddDbContext<NexaWorkDbIdentityContext>(options =>
        // {
        //     options.UseOpenIddict();
        //     // Assuming you have your connection string config here too
        // });



        // services.AddIdentity<NexaWorkUser, NexaWorkRole>(options =>
        // {
        //     // Sign-in settings
        //     options.SignIn.RequireConfirmedAccount = false; // Cho phép đăng nhập mà không cần xác nhận email
        //     // Password settings
        //     options.Password.RequireDigit = true;
        //     options.Password.RequiredLength = 8;
        //     options.Password.RequireNonAlphanumeric = false;
        //     options.Password.RequireUppercase = true;
        //     options.Password.RequireLowercase = true;
        //     // Lockout settings
        //     options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
        //     options.Lockout.MaxFailedAccessAttempts = 5;
        //     options.Lockout.AllowedForNewUsers = true;
        //     // User settings
        //     options.User.RequireUniqueEmail = true;
        //     // Sign-in settings
        //     options.SignIn.RequireConfirmedEmail = false;  // Sign in không cần confirm email
        //     options.SignIn.RequireConfirmedPhoneNumber = false;
        // })
        // .AddEntityFrameworkStores<NexaWorkDbIdentityContext>()
        // .AddDefaultTokenProviders();





        // Cấu hình OpenIddict
        // services.AddOpenIddict()
        //     // Register the OpenIddict core components.
        //     .AddCore(options =>
        //     {
        //         // Configure OpenIddict to use the Entity Framework Core stores and models.
        //         options.UseEntityFrameworkCore()
        //                .UseDbContext<NexaWorkDbIdentityContext>();
        //     })
        //     // Register the OpenIddict server components.
        //     .AddServer(options =>
        //     {
        //         // Enable the authorization and token endpoints.
        //         options.SetAuthorizationEndpointUris("connect/authorize")
        //                .SetTokenEndpointUris("connect/token");

        //         // Enable the Authorization Code Flow with PKCE (The most secure standard)
        //         options.AllowAuthorizationCodeFlow();

        //         options.AllowPasswordFlow();

        //         // Register the signing and encryption credentials.
        //         // NOTE: For development, we use ephemeral keys. For production, you need a real certificate.
        //         options.AddDevelopmentEncryptionCertificate()
        //                .AddDevelopmentSigningCertificate();

        //         // Register the ASP.NET Core host and configure the options.
        //         options.UseAspNetCore()
        //                .EnableAuthorizationEndpointPassthrough()
        //                .EnableTokenEndpointPassthrough();
        //     })
        //     // Register the OpenIddict validation components.
        //     .AddValidation(options =>
        //     {
        //         // Import the configuration from the local OpenIddict server instance.
        //         options.UseLocalServer();
        //         options.UseAspNetCore();
        //     });

        #endregion




        // 2. Register the Unit of Work
        // This ensures that when a Handler asks for IApplicationDbContext, 
        // it gets the exact same NexaWorkDbContext instance used by the repositories.
        services.AddScoped<INexaWorkDbContext>(provider =>
            provider.GetRequiredService<NexaWorkDbContext>());

        // 3. Register your Repositories
        // We use AddScoped so a new instance is created once per HTTP request.
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
        services.AddScoped<ICustomerSocialLinkRepository, CustomerSocialLinkRepository>();
        services.AddScoped<IEducationRepository, EducationRepository>();
        services.AddScoped<IReactionRepository, ReactionRepository>();

        // 4. Register your Services
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        return services;
    }
}
