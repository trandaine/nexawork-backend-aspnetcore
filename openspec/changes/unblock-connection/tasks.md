# Task Breakdown: Unblock Connection (`unblock-connection`)

- **Change ID**: `unblock-connection`
- **Status**: Draft / Proposed
- **Author**: Backend Team
- **Created Date**: 2026-08-26

---

## Task Matrix

### Milestone 1: Domain & Infrastructure Foundation
- [x] **Task 1.1**: Add `StatusBeforeBlock` (`ConnectionStatus?`) property to `NexaWork.Domain/Entities/Connection.cs`.
- [x] **Task 1.2**: Update `ConnectionConfiguration.cs` in `NexaWork.Infrastructure/Persistence/Configurations/` to configure `StatusBeforeBlock` as optional.
- [x] **Task 1.3**: Add EF Core migration `AddStatusBeforeBlock` for `NexaWorkDbContext` (`dotnet ef migrations add AddStatusBeforeBlock -s NexaWork.Client -p NexaWork.Infrastructure -c NexaWorkDbContext`).
- [x] **Task 1.4**: Apply migration to database (`dotnet ef database update -s NexaWork.Client -p NexaWork.Infrastructure -c NexaWorkDbContext`).

### Milestone 2: Fix Block Connection Handler
- [x] **Task 2.1**: Update `BlockConnectionRequestHandler.cs` to restrict blocking to `ConnectionStatus.Accepted` connections only.
- [x] **Task 2.2**: In `BlockConnectionRequestHandler.cs`, save current status into `StatusBeforeBlock`.
- [x] **Task 2.3**: In `BlockConnectionRequestHandler.cs`, ensure `connection.CustomerId` is set to the blocker's customer ID and `ConnectedCustomerId` to the target.
- [x] **Task 2.4**: Remove the "create new blocked connection record for strangers" fallback branch.

### Milestone 3: Implement Unblock Feature
- [ ] **Task 3.1**: Create `UnblockConnectionCommand.cs` in `NexaWork.Application/Features/Client/Connections/Commands/UnblockConnection/`.
- [ ] **Task 3.2**: Create `UnblockConnectionHandler.cs` with validation: must be blocked, caller must be the blocker (`CustomerId`), revert status to `StatusBeforeBlock` (or default `Accepted`), clear `StatusBeforeBlock`.
- [ ] **Task 3.3**: Create `UnblockConnectionValidator.cs` with FluentValidation rules for `TargetCustomerId`.
- [ ] **Task 3.4**: Add `PUT /api/Connections/{targetCustomerId}/unblock` endpoint to `ConnectionsController.cs`.

### Milestone 4: Verification & Testing
- [ ] **Task 4.1**: Build solution and verify 0 compilation errors/warnings (`dotnet build`).
- [ ] **Task 4.2**: Verify block restriction: attempt to block a user with no connection → throws `InvalidOperationException`.
- [ ] **Task 4.3**: Verify block on accepted connection: status becomes `Blocked`, `StatusBeforeBlock` becomes `Accepted`, `CustomerId` becomes blocker.
- [ ] **Task 4.4**: Verify unblock by non-blocker (blocked user): throws `UnauthorizedAccessException` (403).
- [ ] **Task 4.5**: Verify unblock by blocker: status returns to `Accepted`, `StatusBeforeBlock` is cleared to `null`.
