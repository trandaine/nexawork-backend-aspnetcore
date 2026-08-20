# Technical Design: Real-Time 1-on-1 Messaging (`realtime-messaging`)

- **Change ID**: `realtime-messaging`
- **Status**: Draft / Proposed
- **Author**: Backend Team
- **Created Date**: 2026-08-20

---

## 1. System Architecture & Component Interactions

```
                           +-------------------------------------+
                           |            Frontend Client          |
                           +------------------+------------------+
                                              |
                   1. REST API (POST/GET/PUT/DELETE)   2. SignalR WebSocket (/hubs/chat)
                                              |                      |
                                              v                      v
                           +-------------------------------------+---+
                           |            NexaWork.Client          |   |
                           |  (MessagesController, ChatHub)     |   |
                           +------------------+------------------+   |
                                              |                      |
                                  MediatR Dispatcher                 |
                                              |                      |
                                              v                      |
                           +------------------+------------------+   |
                           |        NexaWork.Application         |   |
                           | (Handlers: Send, Read, Delete, etc.)|   |
                           +--------+-------------------+--------+   |
                                    |                   |            |
                  Check Connection  |                   | Save Msg   | Push Event
                                    v                   v            |
            +-----------------------+--+     +----------+------------+--+
            |      NexaWorkDbContext   |     |      MessageDbContext    |
            |     (NexaWorkDatabase)   |     |      (NexaMessageDB)     |
            +--------------------------+     +--------------------------+
```

---

## 2. Cross-Database Strategy & Data Model

### 2.1 Database Separation
1. **`NexaWorkDatabase`**: Managed by `NexaWorkDbContext`. Contains `Customers`, `Connections`, `Posts`, `Jobs`, etc.
2. **`NexaMessageDB`**: Managed by `MessageDbContext`. Contains `Messages`.

### 2.2 Entity: `Message`
Located at `NexaWork.Domain/Entities/Message.cs`:

```csharp
namespace NexaWork.Domain.Entities;

public class Message
{
    public Guid MessageId { get; set; }
    public Guid SenderCustomerId { get; set; }
    public Guid ReceiverCustomerId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

*Note: No EF navigation properties to `Customer` because the entities reside in separate physical databases.*

### 2.3 MessageDbContext & Global Query Filter
Located at `NexaWork.Infrastructure/Persistence/MessageDbContext.cs`:

```csharp
public class MessageDbContext : DbContext, IMessageDbContext
{
    public DbSet<Message> Messages => Set<Message>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MessageConfiguration());
    }
}
```

Configuration `MessageConfiguration`:
- Primary Key: `MessageId`
- `Content`: `HasMaxLength(2000).IsRequired()`
- Global Query Filter: `builder.HasQueryFilter(m => !m.IsDeleted);`
- Indexes:
  - `(SenderCustomerId, ReceiverCustomerId, CreatedAt DESC)`
  - `(ReceiverCustomerId, SenderCustomerId, CreatedAt DESC)`

---

## 3. CQRS & MediatR Pipeline Design

### 3.1 Feature Layout
```
NexaWork.Application/Features/Client/Messages/
|-- Commands/
|   |-- SendMessage/
|   |   |-- SendMessageCommand.cs
|   |   |-- SendMessageHandler.cs
|   |   +-- SendMessageValidator.cs
|   |-- MarkMessageAsRead/
|   |   |-- MarkMessageAsReadCommand.cs
|   |   |-- MarkMessageAsReadHandler.cs
|   |   +-- MarkMessageAsReadValidator.cs
|   |-- MarkConversationAsRead/
|   |   |-- MarkConversationAsReadCommand.cs
|   |   |-- MarkConversationAsReadHandler.cs
|   |   +-- MarkConversationAsReadValidator.cs
|   +-- DeleteMessage/
|       |-- DeleteMessageCommand.cs
|       |-- DeleteMessageHandler.cs
|       +-- DeleteMessageValidator.cs
+-- Queries/
    +-- GetConversation/
        |-- GetConversationQuery.cs
        |-- GetConversationQueryHandler.cs
        +-- GetConversationValidator.cs
```

### 3.2 Key Handler Logic: `SendMessageHandler`
1. Extract `IdentityUserId` from `ICurrentUserService.UserId`.
2. Retrieve current `Customer` record via `ICustomerRepository.GetByIdentityIdToEditAsync`.
3. Check connection status via `IConnectionRepository.GetConnectionAsync(currentCustomerId, targetCustomerId)`:
   - Must exist.
   - Status must equal `ConnectionStatus.Accepted`.
   - If not accepted or blocked, throw `ForbiddenAccessException` or `BadRequestException`.
4. Instantiate `Message` entity.
5. Save to `IMessageRepository.AddAsync` -> `IMessageDbContext.SaveChangesAsync`.
6. Dispatch notification via `IMessageNotificationService.NotifyNewMessage(receiverCustomerId, messageDto)`.
7. Return generated `MessageId`.

---

## 4. SignalR Real-Time Realization

### 4.1 Interface: `IMessageNotificationService`
Located in `NexaWork.Application/Common/Interfaces/Services/IMessageNotificationService.cs`:

```csharp
public interface IMessageNotificationService
{
    Task NotifyNewMessage(Guid receiverCustomerId, MessageDto message, CancellationToken ct = default);
    Task NotifyMessageRead(Guid senderCustomerId, Guid messageId, DateTime readAt, CancellationToken ct = default);
    Task NotifyMessageDeleted(Guid receiverCustomerId, Guid messageId, CancellationToken ct = default);
}
```

### 4.2 SignalR User Mapping
- `CustomUserIdProvider`: Implements `IUserIdProvider`. Returns the user's `ClaimTypes.NameIdentifier` or `CustomerId`.
- Client subscribes to hub at `/hubs/chat` using Bearer JWT authentication.
- Events emitted:
  - `ReceiveMessage` (payload: `MessageDto`)
  - `MessageRead` (payload: `{ messageId: Guid, readAt: DateTime }`)
  - `MessageDeleted` (payload: `{ messageId: Guid }`)

---

## 5. Security & Validation Boundaries

1. **Authentication**: All endpoints protected with `[Authorize]`.
2. **Authorization & Ownership**:
   - `DeleteMessage`: Validates `message.SenderCustomerId == currentCustomerId`.
   - `MarkMessageAsRead`: Validates `message.ReceiverCustomerId == currentCustomerId`.
3. **Friendship Enforcement**: Messages strictly rejected unless `ConnectionStatus == Accepted`.
4. **Input Constraints**: Content 1-2000 characters, non-empty receiver GUID, cannot self-message.
