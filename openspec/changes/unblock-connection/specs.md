# Specification: Unblock Connection (`unblock-connection`)

- **Change ID**: `unblock-connection`
- **Status**: Draft / Proposed
- **Author**: Backend Team
- **Created Date**: 2026-08-26

---

## 1. Domain Specification: `Connection`

### Entity Definition
- **Class**: `NexaWork.Domain.Entities.Connection`
- **Database**: `NexaWorkDatabase`
- **Table Name**: `Connections`

### Attributes & Rules
```csharp
public class Connection
{
    public Guid ConnectionId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ConnectedCustomerId { get; set; }
    public ConnectionStatus Status { get; set; }
    public ConnectionStatus? StatusBeforeBlock { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Customer Customer { get; set; } = null!;
    public virtual Customer ConnectedCustomer { get; set; } = null!;
}
```

### Constraints & Invariants
1. `ConnectionId`: Primary key (UUID).
2. `CustomerId`: GUID of connection initiator or blocker.
3. `ConnectedCustomerId`: GUID of target/receiver or blocked user.
4. `Status`: Enum (`Pending = 0`, `Accepted = 1`, `Rejected = 2`, `Blocked = 3`).
5. `StatusBeforeBlock`: Nullable enum (`ConnectionStatus?`). Populated when `Status == Blocked` with the pre-block status (typically `Accepted = 1`). Set to `null` on unblock or when not blocked.

---

## 2. API Contract Specification

Base path: `/api/Connections`  
Header: `Authorization: Bearer <JWT>`

### 2.1 Unblock Connection
- **Method**: `PUT`
- **Path**: `/api/Connections/{targetCustomerId}/unblock`
- **Route Parameters**:
  - `targetCustomerId` (Guid, required): The ID of the blocked user to unblock.
- **Request Body**: None.
- **Rules**:
  - Authenticated user must be the blocker (`connection.CustomerId == currentCustomerId`).
  - Connection must exist and its status must be `ConnectionStatus.Blocked`.
  - On success, connection status is restored to `StatusBeforeBlock` (or `Accepted`), and `StatusBeforeBlock` is cleared to `null`.
- **Responses**:
  - `204 NoContent`: Success. Connection restored.
  - `400 Bad Request`: Validation failure (empty/invalid target GUID, self-unblock attempt, or connection not in `Blocked` status).
  - `401 Unauthorized`: Missing or invalid JWT.
  - `403 Forbidden`: Authenticated user is the blocked party, not the blocker.
  - `404 Not Found`: Target user or connection record not found.

### 2.2 Updated Block Connection Behavior
- **Method**: `PUT`
- **Path**: `/api/Connections/{targetCustomerId}/block`
- **Updated Rules**:
  - Connection between current user and `targetCustomerId` must already exist with `Status == ConnectionStatus.Accepted`.
  - If no connection exists or status is `Pending`/`Rejected`, the request is rejected with `400 Bad Request` / `InvalidOperationException`.
  - Sets `connection.StatusBeforeBlock = connection.Status` (`Accepted`).
  - Sets `connection.CustomerId = currentCustomerId` (blocker becomes owner).
  - Sets `connection.ConnectedCustomerId = targetCustomerId`.
  - Sets `connection.Status = ConnectionStatus.Blocked`.

---

## 3. Real-Time Events Specification

Unblocking is a private, silent action. **No SignalR events are emitted** upon unblocking or blocking connections.
