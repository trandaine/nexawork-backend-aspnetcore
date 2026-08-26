# Task Breakdown: Real-Time 1-on-1 Messaging (`realtime-messaging`)

- **Change ID**: `realtime-messaging`
- **Status**: Archived / Implemented
- **Author**: Backend Team
- **Created Date**: 2026-08-20

---

## Task Matrix

### Milestone 1: Domain & Database Foundation
- [x] **Task 1.1**: Create `Message` entity in `NexaWork.Domain/Entities/Message.cs`.
- [x] **Task 1.2**: Define `IMessageDbContext` interface in `NexaWork.Application/Common/Interfaces/IMessageDbContext.cs`.
- [x] **Task 1.3**: Create `MessageConfiguration` in `NexaWork.Infrastructure/Persistence/Configurations/MessageConfiguration.cs` with indexes and soft delete query filter.
- [x] **Task 1.4**: Implement `MessageDbContext` in `NexaWork.Infrastructure/Persistence/MessageDbContext.cs`.
- [x] **Task 1.5**: Update `ConnectionStringConstants.cs` and `appsettings.json` with `MessageConnection` connection string.

### Milestone 2: Repository & Infrastructure
- [x] **Task 2.1**: Define `IMessageRepository` interface in `NexaWork.Application/Common/Interfaces/Repositories/IMessageRepository.cs`.
- [x] **Task 2.2**: Implement `MessageRepository` in `NexaWork.Infrastructure/Persistence/Repositories/MessageRepository.cs`.
- [x] **Task 2.3**: Register `MessageDbContext`, `IMessageDbContext`, and `IMessageRepository` in `NexaWork.Infrastructure/DependencyInjection.cs` and `NexaWork.Client/Program.cs`.
- [x] **Task 2.4**: Generate initial EF Core migration for `MessageDbContext` (`InitialMessageDb`).

### Milestone 3: Application CQRS Features & Validation
- [x] **Task 3.1**: Create `MessageDto` in `NexaWork.Application/DTOs/Messages/MessageDto.cs`.
- [x] **Task 3.2**: Implement `SendMessage` (Command, Handler, Validator). Enforce `ConnectionStatus.Accepted` check.
- [x] **Task 3.3**: Implement `GetConversation` (Query, Handler, Validator) with pagination and sender detail hydration.
- [x] **Task 3.4**: Implement `MarkMessageAsRead` (Command, Handler, Validator).
- [x] **Task 3.5**: Implement `MarkConversationAsRead` (Command, Handler, Validator).
- [x] **Task 3.6**: Implement `DeleteMessage` (Command, Handler, Validator) with soft deletion logic.

### Milestone 4: Real-Time SignalR Implementation
- [x] **Task 4.1**: Define `IMessageNotificationService` interface in `NexaWork.Application/Common/Interfaces/Services/IMessageNotificationService.cs`.
- [x] **Task 4.2**: Implement `ChatHub` in `NexaWork.Client/Hubs/ChatHub.cs`.
- [x] **Task 4.3**: Implement `CustomUserIdProvider` in `NexaWork.Client/Hubs/CustomUserIdProvider.cs`.
- [x] **Task 4.4**: Implement `MessageNotificationService` in `NexaWork.Client/Services/MessageNotificationService.cs`.
- [x] **Task 4.5**: Register SignalR and map `/hubs/chat` in `NexaWork.Client/Program.cs`.

### Milestone 5: Web API & Verification
- [x] **Task 5.1**: Implement `MessagesController` in `NexaWork.Client/Controllers/MessagesController.cs`.
- [x] **Task 5.2**: Build solution and verify clean compilation without warnings/errors (`dotnet build`).
- [x] **Task 5.3**: Perform automated / manual API verification for message sending, read status, deletion, and connection validation gates.
