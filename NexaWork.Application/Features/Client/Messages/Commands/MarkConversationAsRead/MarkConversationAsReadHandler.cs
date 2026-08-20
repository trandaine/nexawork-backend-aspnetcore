using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;
using NexaWork.Domain.Entities;

namespace NexaWork.Application.Features.Client.Messages.Commands.MarkConversationAsRead;

public class MarkConversationAsReadHandler : IRequestHandler<MarkConversationAsReadCommand>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMessageDbContext _messageDbContext;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMessageNotificationService _notificationService;

    public MarkConversationAsReadHandler(
        IMessageRepository messageRepository,
        IMessageDbContext messageDbContext,
        ICustomerRepository customerRepository,
        ICurrentUserService currentUserService,
        IMessageNotificationService notificationService)
    {
        _messageRepository = messageRepository;
        _messageDbContext = messageDbContext;
        _customerRepository = customerRepository;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
    }

    public async Task Handle(MarkConversationAsReadCommand request, CancellationToken cancellationToken)
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

        var unreadMessages = await _messageRepository.GetUnreadMessagesFromSenderAsync(
            currentCustomer.CustomerId, 
            request.OtherCustomerId, 
            cancellationToken);

        if (unreadMessages.Count == 0)
        {
            return;
        }

        var readAt = DateTime.UtcNow;
        foreach (var msg in unreadMessages)
        {
            msg.IsRead = true;
            msg.ReadAt = readAt;
            _messageRepository.Update(msg);
        }

        await _messageDbContext.SaveChangesAsync(cancellationToken);

        // Notify sender for each read message
        foreach (var msg in unreadMessages)
        {
            await _notificationService.NotifyMessageRead(msg.SenderCustomerId, msg.MessageId, readAt, cancellationToken);
        }
    }
}
