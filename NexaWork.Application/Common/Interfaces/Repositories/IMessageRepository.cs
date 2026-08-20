using NexaWork.Domain.Entities;

namespace NexaWork.Application.Common.Interfaces.Repositories;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(Guid messageId, CancellationToken cancellationToken = default);
    Task<List<Message>> GetConversationAsync(Guid customerId, Guid otherCustomerId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(Message message, CancellationToken cancellationToken = default);
    void Update(Message message);
    Task<List<Message>> GetUnreadMessagesFromSenderAsync(Guid receiverCustomerId, Guid senderCustomerId, CancellationToken cancellationToken = default);
}
