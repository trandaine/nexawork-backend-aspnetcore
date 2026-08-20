using MediatR;

namespace NexaWork.Application.Features.Client.Messages.Commands.DeleteMessage;

public record DeleteMessageCommand(Guid MessageId) : IRequest;
