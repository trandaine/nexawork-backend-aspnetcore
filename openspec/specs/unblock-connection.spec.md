# OpenSpec: Unblock Connection Feature (`unblock-connection`)

- **Feature Name**: `unblock-connection`
- **Status**: Proposed / In Review
- **Owner**: Backend Team
- **Target Release**: v1.2.0
- **Last Updated**: 2026-08-26

---

## 1. Executive Summary & Goals

### 1.1 Context & Background
NexaWork currently enables users to block an existing connection via `PUT /api/Connections/{targetCustomerId}/block`. However, there is no corresponding endpoint to unblock a connection. Furthermore, the block logic allows blocking non-connected users (strangers) and fails to preserve the pre-block connection status.

### 1.2 Objective
Provide a clean, secure unblock mechanism that:
1. Reverts a previously blocked connection back to its `Accepted` state.
2. Restricts the unblock action exclusively to the user who initiated the block.
3. Fixes the block handler to only permit blocking active `Accepted` connections (rejecting attempts on strangers or Pending/Rejected connections).
4. Persists the pre-block status via a new `StatusBeforeBlock` column on the `Connection` entity.

### 1.3 Key Requirements
- **Unblock Endpoint**: `PUT /api/Connections/{targetCustomerId}/unblock`.
- **Status Restoration**: Restores `ConnectionStatus` to `Accepted` and clears `StatusBeforeBlock`.
- **Authorization Enforcement**: Only the blocker (`connection.CustomerId`) can unblock. Blocked users cannot unblock themselves.
- **Block Restriction**: Blocking is strictly limited to existing `Accepted` connections. Attempts on strangers or non-Accepted connections throw `InvalidOperationException`.
- **Database Schema**: Add `StatusBeforeBlock` (`ConnectionStatus?`, nullable) to `Connections` table in `NexaWorkDatabase` via EF Core migration.
- **Clean Architecture & CQRS**: MediatR command/handler, FluentValidation pipeline behavior, Repository pattern.

---

## 2. Architecture & Domain Model

### 2.1 State Machine Transitions

```
               Send Request
   [No Record] -------------> [ Pending ]
                                 |   |
                    Accept       |   | Reject
         +-----------------------+   +--------------------+
         |                                                |
         v                                                v
    [ Accepted ]                                     [ Rejected ]
         |   ^
   Block |   | Unblock (Blocker only)
         v   |
    [ Blocked ]
    (StatusBeforeBlock = Accepted)
```

### 2.2 Data Schema Specification: `Connection`
**Assembly / Namespace**: `NexaWork.Domain.Entities.Connection`  
**Database**: `NexaWorkDatabase` (`NexaWorkDbContext`)  
**Table**: `Connections`

| Column Name | Type | Nullable | Description |
|-------------|------|----------|-------------|
| `ConnectionId` | `UNIQUEIDENTIFIER` | No | PK, unique connection identifier |
| `CustomerId` | `UNIQUEIDENTIFIER` | No | Connection initiator / Blocker customer ID |
| `ConnectedCustomerId` | `UNIQUEIDENTIFIER` | No | Target / Blocked customer ID |
| `Status` | `INT` | No | Current status (`Pending=0`, `Accepted=1`, `Rejected=2`, `Blocked=3`) |
| `StatusBeforeBlock` | `INT` | Yes | Pre-block status (`Accepted=1`). Null when not blocked. |
| `CreatedAt` | `DATETIME2` | No | Connection creation timestamp (UTC) |

---

## 3. API Endpoints Specification

Base Route: `/api/Connections`  
Authentication: Bearer JWT Token (`[Authorize]`)

### 3.1 `PUT /api/Connections/{targetCustomerId}/unblock` — Unblock Connection
- **Description**: Restores a previously blocked connection back to `Accepted`.
- **Route Parameter**: `targetCustomerId` (Guid, required) — The ID of the blocked customer.
- **Authorization**: Caller must be the blocker (`connection.CustomerId == currentUser.CustomerId`).
- **Validation Rules**:
  - `targetCustomerId` must be a valid, non-empty GUID.
  - `targetCustomerId` cannot equal the caller's Customer ID.
  - Connection must exist in `Blocked` status.
- **Responses**:
  - `204 NoContent`: Connection successfully unblocked and restored to `Accepted`.
  - `400 Bad Request`: Validation error or connection is not in `Blocked` state.
  - `401 Unauthorized`: Missing / invalid JWT bearer token.
  - `403 Forbidden`: Authenticated user is the blocked party (not the blocker).
  - `404 Not Found`: Target user or connection record does not exist.

### 3.2 `PUT /api/Connections/{targetCustomerId}/block` — Block Connection (Updated)
- **Description**: Blocks an existing accepted connection.
- **Route Parameter**: `targetCustomerId` (Guid, required).
- **Validation Rules**:
  - Connection must already exist with `Status == ConnectionStatus.Accepted`.
  - Attempts to block strangers or non-Accepted connections return `400 Bad Request` / `InvalidOperationException`.
- **Side Effects**:
  - `connection.StatusBeforeBlock = connection.Status` (`Accepted`).
  - `connection.CustomerId = currentUser.CustomerId` (ensures blocker is the record owner).
  - `connection.ConnectedCustomerId = targetCustomerId`.
  - `connection.Status = ConnectionStatus.Blocked`.
- **Responses**:
  - `204 NoContent`: Connection successfully blocked.
  - `400 Bad Request`: Cannot block self, or connection does not exist / is not `Accepted`.
  - `404 Not Found`: Target user does not exist.

---

## 4. Implementation Checklist & File Plan

- [ ] `NexaWork.Domain/Entities/Connection.cs` (Add `StatusBeforeBlock`)
- [ ] `NexaWork.Infrastructure/Persistence/Configurations/ConnectionConfiguration.cs` (Configure `StatusBeforeBlock`)
- [ ] EF Core Migration `AddStatusBeforeBlock` for `NexaWorkDbContext`
- [ ] `NexaWork.Application/Features/Client/Connections/Commands/BlockConnection/BlockConnectionRequestHandler.cs` (Update logic)
- [ ] `NexaWork.Application/Features/Client/Connections/Commands/UnblockConnection/UnblockConnectionCommand.cs`
- [ ] `NexaWork.Application/Features/Client/Connections/Commands/UnblockConnection/UnblockConnectionHandler.cs`
- [ ] `NexaWork.Application/Features/Client/Connections/Commands/UnblockConnection/UnblockConnectionValidator.cs`
- [ ] `NexaWork.Client/Controllers/ConnectionsController.cs` (Add unblock action)
