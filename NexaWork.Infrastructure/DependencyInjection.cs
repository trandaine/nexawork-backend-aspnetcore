using Microsoft.Extensions.DependencyInjection;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Infrastructure.Persistence;
using NexaWork.Infrastructure.Persistence.Repositories;

namespace NexaWork.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services 
    // IConfiguration configuration
    )
    {
        // 1. Register the DbContext (Your Database Connection)
        // services.AddDbContext<NexaWorkDbContext>(options =>
        //     options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
        //         builder => builder.MigrationsAssembly(typeof(NexaWorkDbContext).Assembly.FullName)));
        services.AddDbContext<NexaWorkDbContext>();
        services.AddDbContext<NexaWorkDbIdentityContext>();

        // 2. Register the Unit of Work
        // This ensures that when a Handler asks for IApplicationDbContext, 
        // it gets the exact same NexaWorkDbContext instance used by the repositories.
        services.AddScoped<INexaWorkDbContext>(provider =>
            provider.GetRequiredService<NexaWorkDbContext>());

        // 3. Register your Repositories
        // We use AddScoped so a new instance is created once per HTTP request.
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();

        return services;
    }
}
