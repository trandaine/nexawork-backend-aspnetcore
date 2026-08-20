using MediatR;

namespace NexaWork.Application.Features.Client.Messages.Commands.MarkMessageAsRead;

public record MarkMessageAsReadCommand(Guid MessageId) : IRequest;
