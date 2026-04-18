using System;
using FluentValidation;
using NexaWork.Application.Features.Client.Organization.Commands.Create;
using NexaWork.Client.Extensions.Services;

namespace NexaWork.Client.Extensions;

public static class ApplicationServiceExtensions
{
    /// <summary>
    /// Add registered services inside Services folder
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddMediatRServices()
            .AddValidationServices();



        return services;
    }


}
