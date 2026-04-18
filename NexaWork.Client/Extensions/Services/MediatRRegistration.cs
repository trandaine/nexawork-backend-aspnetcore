using System;
using System.Reflection;
using NexaWork.Application.Features.Client.Organization.Commands.Create;

namespace NexaWork.Client.Extensions.Services;

public static class MediatRRegistration
{
    /// <summary>
    /// Registers MediatR services by scanning the assembly for handlers, requests, and notifications. 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMediatRServices(this IServiceCollection services)
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
