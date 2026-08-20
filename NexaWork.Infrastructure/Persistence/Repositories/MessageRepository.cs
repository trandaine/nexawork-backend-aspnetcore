using Microsoft.EntityFrameworkCore;
using NexaWork.Application.Common.Interfaces;
using NexaWork.Application.Common.Interfaces.Repositories;
using NexaWork.Domain.Entities;
using NexaWork.Infrastructure.Persistence;

namespace NexaWork.Infrastructure.Persistence.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly MessageDbContext _context;

    public MessageRepository(MessageDbContext context)
    {
        _context = context;
    }

    public async Task<Message?> GetByIdAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .FirstOrDefaultAsync(m => m.MessageId == messageId, cancellationToken);
    }

    public async Task<List<Message>> GetConversationAsync(
        Guid customerId, 
        Guid otherCustomerId, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Where(m => (m.SenderCustomerId == customerId && m.ReceiverCustomerId == otherCustomerId) ||
                        (m.SenderCustomerId == otherCustomerId && m.ReceiverCustomerId == customerId))
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Message message, CancellationToken cancellationToken = default)
    {
        await _context.Messages.AddAsync(message, cancellationToken);
    }

    public void Update(Message message)
    {
        _context.Messages.Update(message);
    }

    public async Task<List<Message>> GetUnreadMessagesFromSenderAsync(
        Guid receiverCustomerId, 
        Guid senderCustomerId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Where(m => m.ReceiverCustomerId == receiverCustomerId &&
                        m.SenderCustomerId == senderCustomerId &&
                        !m.IsRead)
            .ToListAsync(cancellationToken);
    }
}
