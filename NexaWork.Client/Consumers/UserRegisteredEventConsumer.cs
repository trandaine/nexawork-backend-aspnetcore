using MassTransit;
using MediatR;
using NexaWork.Application.Features.Client.Customers.Commands.Create;
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

        // Run your existing business logic!
        var command = new CreateCustomerCommand(context.Message.UserId);
        await _mediator.Send(command);

        _logger.LogInformation("Successfully created Customer profile for User ID: {UserId}", context.Message.UserId);
    }
}
