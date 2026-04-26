using System;
using Microsoft.Extensions.DependencyInjection;
using NexaWork.Application.Features.Client.Organization.Commands.Create;

namespace NexaWork.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            // Register the MediatR services commands
            cfg.RegisterServicesFromAssembly(typeof(CreateOrganizationCommand).Assembly);

            // cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        
        
        
        return services;
    }
}
