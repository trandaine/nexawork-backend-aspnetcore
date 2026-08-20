using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;
using NexaWork.Application.DTOs.Messages;
using NexaWork.Domain.Entities;
using NexaWork.Domain.Enums;

namespace NexaWork.Application.Features.Client.Messages.Commands.SendMessage;

public class SendMessageHandler : IRequestHandler<SendMessageCommand, MessageDto>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMessageDbContext _messageDbContext;
    private readonly ICustomerRepository _customerRepository;
    private readonly IConnectionRepository _connectionRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMessageNotificationService _notificationService;

    public SendMessageHandler(
        IMessageRepository messageRepository,
        IMessageDbContext messageDbContext,
        ICustomerRepository customerRepository,
        IConnectionRepository connectionRepository,
        ICurrentUserService currentUserService,
        IMessageNotificationService notificationService)
    {
        _messageRepository = messageRepository;
        _messageDbContext = messageDbContext;
        _customerRepository = customerRepository;
        _connectionRepository = connectionRepository;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
    }

    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var identityUserId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(identityUserId))
        {
            throw new UnauthorizedAccessException("Người dùng chưa đăng nhập.");
        }

        var sender = await _customerRepository.GetByIdentityIdToEditAsync(identityUserId, cancellationToken);
        if (sender == null)
        {
            throw new KeyNotFoundException("Customer profile not found for current user.");
        }

        if (sender.CustomerId == request.ReceiverCustomerId)
        {
            throw new InvalidOperationException("Bạn không thể tự gửi tin nhắn cho chính mình.");
        }

        var receiver = await _customerRepository.GetCustomerByIdAsync(request.ReceiverCustomerId, cancellationToken);
        if (receiver == null)
        {
            throw new KeyNotFoundException("Receiver customer not found.");
        }

        // Check connection gate
        var connection = await _connectionRepository.GetConnectionAsync(sender.CustomerId, request.ReceiverCustomerId, cancellationToken);
        if (connection == null || connection.Status != ConnectionStatus.Accepted)
        {
            throw new InvalidOperationException("Bạn chỉ có thể gửi tin nhắn cho người dùng đã kết bạn.");
        }

        var message = new Message
        {
            MessageId = Guid.NewGuid(),
            SenderCustomerId = sender.CustomerId,
            ReceiverCustomerId = request.ReceiverCustomerId,
            Content = request.Content.Trim(),
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _messageRepository.AddAsync(message, cancellationToken);
        await _messageDbContext.SaveChangesAsync(cancellationToken);

        var dto = new MessageDto
        {
            MessageId = message.MessageId,
            SenderCustomerId = sender.CustomerId,
            SenderFirstName = sender.FirstName,
            SenderLastName = sender.LastName,
            SenderProfilePictureUrl = sender.ProfilePictureUrl,
            ReceiverCustomerId = message.ReceiverCustomerId,
            Content = message.Content,
            IsRead = message.IsRead,
            CreatedAt = message.CreatedAt
        };

        // Realtime notification
        await _notificationService.NotifyNewMessage(request.ReceiverCustomerId, dto, cancellationToken);

        return dto;
    }
}
