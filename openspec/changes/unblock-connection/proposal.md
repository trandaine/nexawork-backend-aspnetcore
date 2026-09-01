# Change Proposal: Unblock Connection (`unblock-connection`)

- **Change ID**: `unblock-connection`
- **Status**: Implemented
- **Author**: Backend Team
- **Created Date**: 2026-08-26

---

## 1. Problem Statement & Motivation
In NexaWork, users can manage professional connections through various states (`Pending`, `Accepted`, `Rejected`, `Blocked`). Currently, a user can block another user via `PUT /api/Connections/{targetCustomerId}/block`. However:

1. **No Unblock Mechanism**: There is no endpoint or command to unblock a previously blocked connection. Once blocked, users are permanently stuck in the `Blocked` state with no way to restore their connection.
2. **Missing Block Restrictions**: The existing `BlockConnectionRequestHandler` allows users to block arbitrary users (strangers with no prior connection), which creates orphan blocked records without a clear previous relationship. Business rules require that blocking must only be allowed for users with an existing `Accepted` connection.
3. **Loss of Previous State**: When a connection is blocked, its original status is overwritten and lost, preventing accurate restoration upon unblocking.

To provide a complete connection lifecycle, NexaWork requires a robust unblock feature that restores the previous `Accepted` connection state and enforces strict authorization (only the blocker can unblock).

## 2. Proposed Solution
1. **Add `StatusBeforeBlock` Column**: Add a nullable `ConnectionStatus?` column to the `Connection` entity in `NexaWork.Domain` and generate an EF Core migration for `NexaWorkDbContext`.
2. **Fix `BlockConnection` Logic**:
   - Restrict blocking exclusively to connections with `Status == ConnectionStatus.Accepted`. Reject attempts to block strangers or non-Accepted connections with an appropriate exception.
   - Store the current status (`Accepted`) in `StatusBeforeBlock` before setting `Status = ConnectionStatus.Blocked`.
   - Ensure the blocker's customer ID is assigned as `CustomerId` (the owning side of the block) and the blocked user is `ConnectedCustomerId`.
3. **Implement `UnblockConnection` Command & Endpoint**:
   - MediatR Command: `UnblockConnectionCommand(Guid TargetCustomerId) : IRequest, IUserRequest`.
   - Handler: Look up the connection between the current user and target user. Validate that the connection is in `Blocked` status and that the current user is the original blocker (`CustomerId == currentUser.CustomerId`).
   - Restore `Status = StatusBeforeBlock` (or default to `Accepted`), then clear `StatusBeforeBlock = null`.
   - Controller Endpoint: `PUT /api/Connections/{targetCustomerId}/unblock`.
4. **Validation & Pipeline**: Implement `UnblockConnectionValidator` ensuring valid, non-empty `TargetCustomerId` that does not match the caller.

## 3. Scope & Boundaries

### In Scope
- Domain entity modification: Add `StatusBeforeBlock` (`ConnectionStatus?`) to `Connection`.
- EF Core entity configuration update & migration for `NexaWorkDbContext`.
- Fix `BlockConnectionRequestHandler` to enforce `Accepted`-only restriction, save `StatusBeforeBlock`, and set blocker ownership.
- CQRS Command, Handler, and FluentValidation for `UnblockConnection`.
- REST endpoint `PUT /api/Connections/{targetCustomerId}/unblock` in `ConnectionsController`.

### Out of Scope
- SignalR real-time notification on unblock (unblocking is a silent action).
- Frontend UI components / pages for unblock (separate frontend task).
- Blocking non-connected users / strangers (explicitly rejected by business rules).

## 4. Impact Analysis & Tradeoffs

| Area | Impact / Decision | Tradeoff / Mitigation |
|------|-------------------|-----------------------|
| **Database Schema** | Adding nullable `StatusBeforeBlock` (`INT`, nullable) to `Connections` table | Requires EF Core migration `AddStatusBeforeBlock`. Backward-compatible since column is nullable with no default value required for existing records. |
| **Block Ownership** | Ensuring `CustomerId` is always the blocker when blocking an existing connection | When user B blocks user A on an existing connection where A was `CustomerId`, the record's `CustomerId` and `ConnectedCustomerId` must be swapped so `CustomerId` reflects the blocker. |
| **Status Restoration** | Reverting to `Accepted` on unblock | Per business decision, unblocking restores the friendship to `Accepted`. If `StatusBeforeBlock` is null (e.g. legacy data), default to `Accepted`. |
