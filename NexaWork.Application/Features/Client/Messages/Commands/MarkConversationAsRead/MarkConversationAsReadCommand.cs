using MediatR;

namespace NexaWork.Application.Features.Client.Messages.Commands.MarkConversationAsRead;

public record MarkConversationAsReadCommand(Guid OtherCustomerId) : IRequest;
