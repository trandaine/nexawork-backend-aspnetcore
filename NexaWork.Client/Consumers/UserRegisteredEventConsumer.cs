using MassTransit;
using MediatR;
using NexaWork.Application.Features.Client.CustomerAddress.Commands.Create;
using NexaWork.Application.Features.Client.Customers.Commands.Create;
using NexaWork.Application.Features.Client.CustomerSocialLink.Commands.Create;
using NexaWork.Contracts;

namespace NexaWork.Client.Consumers;

public class UserRegisteredEventConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly IMediator _mediator; // using MediatR (Important), not MassTransit's mediator
    private readonly ILogger<UserRegisteredEventConsumer> _logger;

    public UserRegisteredEventConsumer(IMediator mediator, ILogger<UserRegisteredEventConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        _logger.LogInformation("Received UserRegisteredEvent for User ID: {UserId}", context.Message.UserId);
        try
        {
            // Run your existing business logic!
            var createCustomerCommand = new CreateCustomerCommand(context.Message.UserId);
            var createCustomerAddress = new CreateCustomerAddressCommand(context.Message.UserId);
            var createCustomerSocialLink = new CreateCustomerSocialLinkCommand(context.Message.UserId);

            await _mediator.Send(createCustomerCommand);
            await _mediator.Send(createCustomerAddress);
            await _mediator.Send(createCustomerSocialLink);

            _logger.LogInformation("Successfully created Customer profile for User ID: {UserId}",
                context.Message.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create customer profile for User ID: {UserId}", context.Message.UserId);
            throw;
        }

    }
}