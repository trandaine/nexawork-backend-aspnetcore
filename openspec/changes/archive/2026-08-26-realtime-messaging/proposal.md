# Change Proposal: Real-Time 1-on-1 Messaging (`realtime-messaging`)

- **Change ID**: `realtime-messaging`
- **Status**: Archived / Implemented
- **Author**: Backend Team
- **Created Date**: 2026-08-20

---

## 1. Problem Statement & Motivation
NexaWork currently supports networking connections (`Connection` entity) where users can send, accept, reject, block, or undo connection requests. However, once users establish an `Accepted` connection, there is no in-platform communication mechanism. Users cannot message their connections, exchange inquiries, or coordinate opportunities within the platform.

To build a complete professional social network experience (similar to LinkedIn), NexaWork requires a 1-on-1 direct messaging system with real-time delivery and read status.

## 2. Proposed Solution
Implement a direct 1-on-1 messaging feature with the following core capabilities:
1. **Direct Messaging API**: Authenticated users can send messages to their `Accepted` connections, retrieve conversation histories, and mark messages as read.
2. **Dedicated Database (`NexaMessageDB`)**: Isolate messaging data from the main `NexaWorkDatabase` to ensure high-frequency messaging transactions do not degrade transactional performance of core business domain tables (Profiles, Jobs, Posts).
3. **Real-Time Push (SignalR)**: Utilize ASP.NET Core SignalR hub (`/hubs/chat`) to instantly push incoming messages, read receipts, and message deletion events to active connected clients.
4. **Soft Deletion**: Allow message senders to soft-delete their messages (`IsDeleted = true`, `DeletedAt = UtcNow`) without hard data purge.
5. **Connection Gate Security**: Enforce that messages can only be sent between users with an active `Accepted` connection status who have not blocked each other.

## 3. Scope & Boundaries

### In Scope
- Dedicated EF Core `MessageDbContext` targeting `NexaMessageDB`.
- `Message` entity with soft deletion, read tracking, and timestamps.
- CQRS commands & queries via MediatR (`SendMessage`, `GetConversation`, `MarkMessageAsRead`, `MarkConversationAsRead`, `DeleteMessage`).
- FluentValidation pipeline validation for all message commands.
- ASP.NET Core SignalR `ChatHub` with custom `IUserIdProvider` for real-time notifications.
- REST controller `MessagesController` (`/api/messages`).
- EF Core database migration for `NexaMessageDB`.

### Out of Scope
- Group chats / multi-user channels (future enhancement).
- Multimedia file attachments / audio messages in chat (future enhancement).
- Typing indicators & presence / online status tracking (future enhancement).
- End-to-End Encryption (E2EE) (future enhancement).

## 4. Impact Analysis & Tradeoffs

| Area | Impact / Decision | Tradeoff / Mitigation |
|------|-------------------|-----------------------|
| **Database Isolation** | Standalone database `NexaMessageDB` | No cross-database foreign key constraints at database engine level. Relationship integrity (checking connection status) must be validated in Application handlers via `IConnectionRepository`. |
| **Real-Time Transport** | SignalR Hub used exclusively for pushing server-to-client events; client-to-server writes go through REST endpoints. | Avoids duplicating validation/pipeline logic across SignalR hubs and MediatR; preserves consistent HTTP error handling and middleware execution. |
| **Data Retention** | Soft deletion via EF Core Global Query Filter | Soft-deleted records remain in DB for audit/compliance while automatically excluded from application queries unless explicitly bypassed. |
