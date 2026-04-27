using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NexaWork.Application.Common.Behaviors;
using NexaWork.Application.Features.Client.Organization.Commands.Create;
using NexaWork.Application.Features.Client.Organization.Commands.Update;

namespace NexaWork.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            // Register the MediatR services commands
            // cfg.RegisterServicesFromAssembly(typeof(CreateOrganizationCommand).Assembly);
            // cfg.RegisterServicesFromAssembly(typeof(UpdateOrganizationCommand).Assembly);

            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // services.AddValidatorsFromAssembly(typeof(CreateOrganizationCommand).Assembly);
        // services.AddValidatorsFromAssembly(typeof(UpdateOrganizationCommand).Assembly);
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));




        return services;
    }
}
