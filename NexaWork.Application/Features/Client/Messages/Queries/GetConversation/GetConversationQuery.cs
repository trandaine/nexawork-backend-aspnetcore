using MediatR;
using NexaWork.Application.DTOs.Messages;

namespace NexaWork.Application.Features.Client.Messages.Queries.GetConversation;

public record GetConversationQuery(
    Guid OtherCustomerId,
    int Page = 1,
    int PageSize = 20
) : IRequest<List<MessageDto>>;
