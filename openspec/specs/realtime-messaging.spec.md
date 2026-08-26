# OpenSpec: Messaging & Real-Time Chat Feature (1-on-1)

- **Feature Name**: `realtime-messaging`
- **Status**: Archived / Implemented
- **Owner**: Backend Team
- **Target Release**: v1.1.0
- **Last Updated**: 2026-08-20

---

## 1. Executive Summary & Goals

### 1.1 Context & Background
NexaWork is an enterprise LinkedIn clone built with ASP.NET Core Clean Architecture. Currently, users can connect with each other (`Connection` entity with `Pending`, `Accepted`, `Rejected`, `Blocked` statuses). However, there is no direct communication channel between connected users.

### 1.2 Objective
Provide a secure, private, 1-on-1 messaging system enabling connected users (`ConnectionStatus.Accepted`) to communicate in real-time. Messages must reside in a dedicated standalone database (`NexaMessageDB`) to decouple high-volume communication storage from core profile/job-board transactional data.

### 1.3 Key Requirements
- **1-on-1 Messaging**: Users can send text messages to their connections.
- **Connection Gate**: Only users with active `Accepted` connection status and not `Blocked` can exchange messages.
- **Real-Time Delivery**: Real-time push via SignalR (`ChatHub`).
- **Read Receipts**: Tracking message read status (`IsRead`, `ReadAt`).
- **Soft Deletion**: Sender can delete sent messages without permanent data loss (`IsDeleted`, `DeletedAt`).
- **Data Isolation**: Dedicated database (`NexaMessageDB`) with its own EF Core `MessageDbContext`.
- **Clean Architecture & CQRS**: MediatR commands/queries, FluentValidation pipeline, Repository pattern.

---

## 2. Architecture & Domain Model

### 2.1 Cross-Database Topology
```
┌───────────────────────────────────────┐         ┌───────────────────────────────────────┐
│          NexaWorkDatabase             │         │            NexaMessageDB              │
│  (Profile, Connections, Posts, Jobs)  │         │          (Messages, Receipts)         │
├───────────────────────────────────────┤         ├───────────────────────────────────────┤
│ - Customers                           │         │ - Messages                            │
│ - Connections                         │         │   (SenderCustomerId,                  │
│ - NexaWorkDbContext                   │         │    ReceiverCustomerId, Content, ...)  │
│                                       │         │ - MessageDbContext                    │
└───────────────────────────────────────┘         └───────────────────────────────────────┘
                     │                                                ▲
                     │ (Validate Friendship Status)                  │ (Persist & Query)
                     └───────────────────────┬────────────────────────┘
                                             │
                                  ┌──────────────────────┐
                                  │  NexaWork.Client API │
                                  │   (MediatR Handlers) │
                                  └──────────┬───────────┘
                                             │ (Push via WebSocket)
                                             ▼
                                  ┌──────────────────────┐
                                  │    SignalR ChatHub   │
                                  └──────────────────────┘
```

> **Note on Cross-DB Constraints**: `Messages` does NOT have foreign key navigation constraints to `Customer` tables at the database level. Domain integrity and authorization are enforced at the Application handler level via `IConnectionRepository` and `ICustomerRepository`.

---

## 3. Data Schema & Specifications

### 3.1 Entity Specification: `Message`
**Assembly / Namespace**: `NexaWork.Domain.Entities.Message`  
**Database**: `NexaMessageDB`  
**Table**: `Messages`

| Column Name | Type | Nullable | Constraints / Defaults | Description |
|-------------|------|----------|------------------------|-------------|
| `MessageId` | `UNIQUEIDENTIFIER` | No | PK, `NEWID()` / `Guid.NewGuid()` | Unique message ID |
| `SenderCustomerId` | `UNIQUEIDENTIFIER` | No | Indexed | Customer ID of the sender |
| `ReceiverCustomerId`| `UNIQUEIDENTIFIER` | No | Indexed | Customer ID of the recipient |
| `Content` | `NVARCHAR(2000)` | No | Max length 2000 | Text body of the message |
| `IsRead` | `BIT` | No | Default `0` (false) | Read receipt flag |
| `ReadAt` | `DATETIME2` | Yes | Nullable | Timestamp when recipient read the message |
| `IsDeleted` | `BIT` | No | Default `0` (false) | Soft delete flag |
| `DeletedAt` | `DATETIME2` | Yes | Nullable | Timestamp when message was deleted |
| `CreatedAt` | `DATETIME2` | No | Default `SYSUTCDATETIME()` | Creation timestamp (UTC) |

### 3.2 Indexing Strategy
- **Composite Index**: `IX_Messages_Sender_Receiver_CreatedAt` on `(SenderCustomerId, ReceiverCustomerId, CreatedAt DESC)`
- **Composite Index**: `IX_Messages_Receiver_Sender_CreatedAt` on `(ReceiverCustomerId, SenderCustomerId, CreatedAt DESC)`
- **Global Query Filter**: `EF.Property<bool>(m, "IsDeleted") == false` (applied globally on `MessageDbContext`).

---

## 4. API Endpoints Specification

Base Route: `/api/messages`  
Authentication: Bearer JWT Token (`[Authorize]`)

### 4.1 `POST /api/messages` — Send Message
- **Description**: Send a text message to a connected customer.
- **Request Body**:
  ```json
  {
    "receiverCustomerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "content": "Hello, let's catch up!"
  }
  ```
- **Validation Rules**:
  - `receiverCustomerId`: Required, non-empty GUID, cannot be current user's ID.
  - `content`: Required, length between 1 and 2000 characters.
  - Friendship status between sender and receiver must be `Accepted` in `NexaWorkDatabase`.
  - Neither user has blocked the other.
- **Responses**:
  - `201 Created`: Returns `MessageDto` with `Location` header or `{ "messageId": "guid" }`.
  - `400 Bad Request`: Validation failure.
  - `403 Forbidden`: Users are not connected or are blocked.
  - `404 Not Found`: Receiver customer not found.

### 4.2 `GET /api/messages/{otherCustomerId}` — Get Conversation
- **Description**: Retrieve paginated conversation history between authenticated customer and `otherCustomerId`.
- **Query Parameters**:
  - `page` (int, default: 1)
  - `pageSize` (int, default: 20, max: 50)
- **Responses**:
  - `200 OK`: Returns `PaginatedList<MessageDto>` ordered by `CreatedAt DESC`.
  - `403 Forbidden`: Users are not connected.

### 4.3 `PUT /api/messages/{messageId}/read` — Mark Message as Read
- **Description**: Mark a specific received message as read.
- **Rules**: Authenticated user must be the `ReceiverCustomerId`.
- **Responses**:
  - `204 NoContent`: Success.
  - `403 Forbidden`: Authenticated user is not the recipient.
  - `404 Not Found`: Message not found.

### 4.4 `PUT /api/messages/{otherCustomerId}/read-all` — Mark Entire Conversation as Read
- **Description**: Mark all unread messages from `otherCustomerId` sent to the current user as read.
- **Responses**:
  - `204 NoContent`: Success.

### 4.5 `DELETE /api/messages/{messageId}` — Delete Message (Soft Delete)
- **Description**: Soft deletes a message.
- **Rules**: Only the original sender (`SenderCustomerId`) can delete their message.
- **Responses**:
  - `204 NoContent`: Success (`IsDeleted = true`, `DeletedAt = DateTime.UtcNow`).
  - `403 Forbidden`: Current user is not the message sender.
  - `404 Not Found`: Message not found or already deleted.

---

## 5. Real-Time Specification (SignalR)

### 5.1 Hub Configuration
- **Hub Endpoint**: `/hubs/chat`
- **Authentication**: JWT Bearer Token passed via query string `access_token` (standard WebSockets transport) or Authorization Header.
- **User Identifier**: Custom `IUserIdProvider` resolving `ClaimTypes.NameIdentifier` (Identity User ID) mapped to `CustomerId`.

### 5.2 Server-to-Client Events

| Event Name | Target Client(s) | Payload Format | Trigger Condition |
|------------|------------------|----------------|-------------------|
| `ReceiveMessage` | `ReceiverCustomerId` | `MessageDto` | When a new message is successfully persisted. |
| `MessageRead` | `SenderCustomerId` | `{ "messageId": "guid", "readAt": "ISO-8601" }` | When recipient marks message(s) as read. |
| `MessageDeleted` | `ReceiverCustomerId` | `{ "messageId": "guid" }` | When sender soft-deletes a message. |

### 5.3 Client-to-Server Flow
To ensure business integrity, all write operations (sending messages, deleting messages) **MUST** go through REST API endpoints to pass through MediatR pipeline validation. The SignalR Hub is strictly used for real-time pushing and connection state tracking.

---

## 6. Security, Validation & Business Invariants

1. **Identity & Tenant Isolation**: Current user ID extracted exclusively from validated JWT claims (`ICurrentUserService`), never trusted from client request body.
2. **Self-Messaging Prevention**: A user cannot send a message to themselves (`SenderCustomerId != ReceiverCustomerId`).
3. **Anti-Spam & Character Limit**: Message content capped at 2000 characters. Empty/whitespace-only messages rejected by `SendMessageValidator`.
4. **Strict Connection Enforcement**:
   - `Status == ConnectionStatus.Accepted` required.
   - If `Status == ConnectionStatus.Blocked` or `Rejected` or `Pending` or non-existent, message dispatch is rejected with HTTP 403 / Domain Exception.
5. **Soft Delete Privacy**: When `IsDeleted == true`, the message content is hidden from query results via EF Core Global Query Filter.

---

## 7. Test & Verification Plan

### 7.1 Automated Unit / Integration Test Cases
1. **Send Message Validation**:
   - Empty content → `ValidationException`.
   - Content > 2000 characters → `ValidationException`.
   - Target is self → `ValidationException`.
2. **Connection Checks**:
   - Send message to non-connected user → `ForbiddenAccessException`.
   - Send message to blocked user → `ForbiddenAccessException`.
   - Send message to connected friend (`Accepted`) → Successfully saved & SignalR dispatched.
3. **Read Receipt Handling**:
   - Sender attempting to mark their own message as read → `ForbiddenAccessException`.
   - Recipient marking message as read → `IsRead == true`, `ReadAt` populated.
4. **Soft Delete**:
   - Recipient attempting to delete sender's message → `ForbiddenAccessException`.
   - Sender deletes message → `IsDeleted == true`, excluded from subsequent `GetConversationQuery`.

---

## 8. Implementation Checklist & File Plan

- [ ] `NexaWork.Domain/Entities/Message.cs`
- [ ] `NexaWork.Application/Common/Interfaces/IMessageDbContext.cs`
- [ ] `NexaWork.Application/Common/Interfaces/Repositories/IMessageRepository.cs`
- [ ] `NexaWork.Application/Common/Interfaces/Services/IMessageNotificationService.cs`
- [ ] `NexaWork.Application/DTOs/Messages/MessageDto.cs`
- [ ] `NexaWork.Application/Features/Client/Messages/Commands/SendMessage/*` (Command, Handler, Validator)
- [ ] `NexaWork.Application/Features/Client/Messages/Commands/MarkMessageAsRead/*`
- [ ] `NexaWork.Application/Features/Client/Messages/Commands/MarkConversationAsRead/*`
- [ ] `NexaWork.Application/Features/Client/Messages/Commands/DeleteMessage/*` (Command, Handler, Validator)
- [ ] `NexaWork.Application/Features/Client/Messages/Queries/GetConversation/*`
- [ ] `NexaWork.Infrastructure/Persistence/MessageDbContext.cs`
- [ ] `NexaWork.Infrastructure/Persistence/Configurations/MessageConfiguration.cs`
- [ ] `NexaWork.Infrastructure/Persistence/Repositories/MessageRepository.cs`
- [ ] `NexaWork.Infrastructure/DependencyInjection.cs`
- [ ] `NexaWork.Client/Hubs/ChatHub.cs`
- [ ] `NexaWork.Client/Hubs/CustomUserIdProvider.cs`
- [ ] `NexaWork.Client/Services/MessageNotificationService.cs`
- [ ] `NexaWork.Client/Controllers/MessagesController.cs`
- [ ] `NexaWork.Client/Program.cs` & `appsettings.json`
- [ ] EF Core Migration for `MessageDbContext`
