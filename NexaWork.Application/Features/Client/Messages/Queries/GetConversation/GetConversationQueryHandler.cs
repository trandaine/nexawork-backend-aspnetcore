using MediatR;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;
using NexaWork.Application.DTOs.Messages;
using NexaWork.Domain.Entities;
using NexaWork.Domain.Enums;

namespace NexaWork.Application.Features.Client.Messages.Queries.GetConversation;

public class GetConversationQueryHandler : IRequestHandler<GetConversationQuery, List<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IConnectionRepository _connectionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetConversationQueryHandler(
        IMessageRepository messageRepository,
        ICustomerRepository customerRepository,
        IConnectionRepository connectionRepository,
        ICurrentUserService currentUserService)
    {
        _messageRepository = messageRepository;
        _customerRepository = customerRepository;
        _connectionRepository = connectionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<MessageDto>> Handle(GetConversationQuery request, CancellationToken cancellationToken)
    {
        var identityUserId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(identityUserId))
        {
            throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");
        }

        var currentCustomer = await _customerRepository.GetByIdentityIdToEditAsync(identityUserId, cancellationToken);
        if (currentCustomer == null)
        {
            throw new KeyNotFoundException("Customer profile not found for current user.");
        }

        // Check connection gate
        var connection = await _connectionRepository.GetConnectionAsync(currentCustomer.CustomerId, request.OtherCustomerId, cancellationToken);
        if (connection == null || connection.Status != ConnectionStatus.Accepted)
        {
            throw new InvalidOperationException("Bạn chỉ có thể xem tin nhắn với người dùng đã kết bạn.");
        }

        var otherCustomer = await _customerRepository.GetCustomerByIdAsync(request.OtherCustomerId, cancellationToken);
        if (otherCustomer == null)
        {
            throw new KeyNotFoundException("Other customer not found.");
        }

        var messages = await _messageRepository.GetConversationAsync(
            currentCustomer.CustomerId,
            request.OtherCustomerId,
            request.Page,
            request.PageSize,
            cancellationToken);

        var result = messages.Select(m =>
        {
            var isSenderCurrent = m.SenderCustomerId == currentCustomer.CustomerId;
            var sender = isSenderCurrent ? currentCustomer : otherCustomer;

            return new MessageDto
            {
                MessageId = m.MessageId,
                SenderCustomerId = m.SenderCustomerId,
                SenderFirstName = sender.FirstName,
                SenderLastName = sender.LastName,
                SenderProfilePictureUrl = sender.ProfilePictureUrl,
                ReceiverCustomerId = m.ReceiverCustomerId,
                Content = m.Content,
                IsRead = m.IsRead,
                ReadAt = m.ReadAt,
                CreatedAt = m.CreatedAt
            };
        }).ToList();

        return result;
    }
}
