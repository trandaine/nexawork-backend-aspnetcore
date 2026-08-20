using MediatR;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Application.Common.Interfaces.Services;
using NexaWork.Domain.Entities;

namespace NexaWork.Application.Features.Client.Messages.Commands.DeleteMessage;

public class DeleteMessageHandler : IRequestHandler<DeleteMessageCommand>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMessageDbContext _messageDbContext;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMessageNotificationService _notificationService;

    public DeleteMessageHandler(
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

    public async Task Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
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

        var message = await _messageRepository.GetByIdAsync(request.MessageId, cancellationToken);
        if (message == null)
        {
            throw new KeyNotFoundException($"Message with ID {request.MessageId} not found.");
        }

        // Only sender can delete their message
        if (message.SenderCustomerId != currentCustomer.CustomerId)
        {
            throw new UnauthorizedAccessException("Bạn không có quyền xóa tin nhắn này.");
        }

        message.IsDeleted = true;
        message.DeletedAt = DateTime.UtcNow;

        _messageRepository.Update(message);
        await _messageDbContext.SaveChangesAsync(cancellationToken);

        // Notify recipient about message deletion
        await _notificationService.NotifyMessageDeleted(message.ReceiverCustomerId, message.MessageId, cancellationToken);
    }
}
