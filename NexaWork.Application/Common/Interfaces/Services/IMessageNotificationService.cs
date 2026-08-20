using NexaWork.Application.DTOs.Messages;

namespace NexaWork.Application.Common.Interfaces.Services;

public interface IMessageNotificationService
{
    Task NotifyNewMessage(Guid receiverCustomerId, MessageDto message, CancellationToken ct = default);
    Task NotifyMessageRead(Guid senderCustomerId, Guid messageId, DateTime readAt, CancellationToken ct = default);
    Task NotifyMessageDeleted(Guid receiverCustomerId, Guid messageId, CancellationToken ct = default);
}
