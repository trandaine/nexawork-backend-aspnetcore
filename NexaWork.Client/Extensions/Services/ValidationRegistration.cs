using System;
using FluentValidation;
using MediatR;
using NexaWork.Application.Common.Behaviors;
using NexaWork.Application.Features.Client.Organization.Commands.Create;

namespace NexaWork.Client.Extensions.Services;

public static class ValidationRegistration
{
    /// <summary>
    /// Registers validation services by scanning the assembly for FluentValidation validators.
    /// This allows for registration of validators, enabling the use of validation throughout the application. 
    /// Additionally, it registers a pipeline behavior to ensure that validation is executed as part of the MediatR request handling pipeline.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddValidationServices(this IServiceCollection services)
    {
        // Add validations here
        services.AddValidatorsFromAssembly(typeof(CreateOrganizationCommand).Assembly);



        // telling the DI container to insert ValidationBehavior into MediatR’s pipeline for every request/response pair
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));




        return services;
    }
}
