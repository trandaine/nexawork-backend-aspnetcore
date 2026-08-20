using MediatR;
using NexaWork.Application.DTOs.Messages;

namespace NexaWork.Application.Features.Client.Messages.Commands.SendMessage;

public record SendMessageCommand(
    Guid ReceiverCustomerId,
    string Content
) : IRequest<MessageDto>;
