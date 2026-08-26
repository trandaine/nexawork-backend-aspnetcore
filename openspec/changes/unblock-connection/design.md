# Technical Design: Unblock Connection (`unblock-connection`)

- **Change ID**: `unblock-connection`
- **Status**: Draft / Proposed
- **Author**: Backend Team
- **Created Date**: 2026-08-26

---

## 1. System Architecture & Component Interactions

```
                            +-------------------------------------+
                            |            Frontend Client          |
                            +------------------+------------------+
                                               |
                                     PUT /api/Connections/{id}/unblock
                                               |
                                               v
                            +-------------------------------------+
                            |            NexaWork.Client          |
                            |       (ConnectionsController)       |
                            +------------------+------------------+
                                               |
                                   MediatR.Send(command)
                                               |
                                               v
                            +-------------------------------------+
                            |        NexaWork.Application         |
                            |     (UnblockConnectionHandler)      |
                            +------------------+------------------+
                                               |
                                    1. Validate Blocker Ownership
                                    2. Revert Status -> Accepted
                                    3. Clear StatusBeforeBlock
                                               |
                                               v
                            +-------------------------------------+
                            |        NexaWorkDbContext            |
                            |       (NexaWorkDatabase)            |
                            +-------------------------------------+
```

---

## 2. Domain & Data Model Modifications

### 2.1 Entity Modification: `Connection`
Located at `NexaWork.Domain/Entities/Connection.cs`:

```csharp
namespace NexaWork.Domain.Entities;

public class Connection
{
    public Guid ConnectionId { get; set; }
    
    [Description("ID của Customer chủ sở hữu kết nối / người thực hiện hành động chặn khi Status = Blocked")]
    public Guid CustomerId { get; set; }
    
    [Description("ID của Customer được kết nối / người bị chặn khi Status = Blocked")]
    public Guid ConnectedCustomerId { get; set; }
    
    public ConnectionStatus Status { get; set; }
    
    [Description("Trạng thái kết nối trước khi bị chặn để phục hồi khi unblock")]
    public ConnectionStatus? StatusBeforeBlock { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    #region Navigation Properties
    public virtual Customer Customer { get; set; } = null!;
    public virtual Customer ConnectedCustomer { get; set; } = null!; 
    #endregion
}
```

### 2.2 EF Core Configuration: `ConnectionConfiguration`
Located at `NexaWork.Infrastructure/Persistence/Configurations/ConnectionConfiguration.cs`:

```csharp
public class ConnectionConfiguration : IEntityTypeConfiguration<Connection>
{
    public void Configure(EntityTypeBuilder<Connection> builder)
    {
        builder.HasKey(c => c.ConnectionId);

        builder.Property(c => c.StatusBeforeBlock)
               .IsRequired(false);

        builder.HasOne(c => c.Customer)
               .WithMany(u => u.SentConnections)
               .HasForeignKey(c => c.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.ConnectedCustomer)
               .WithMany(u => u.ReceivedConnections)
               .HasForeignKey(c => c.ConnectedCustomerId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
```

---

## 3. CQRS & MediatR Pipeline Design

### 3.1 Feature Layout
```
NexaWork.Application/Features/Client/Connections/Commands/
|-- BlockConnection/
|   |-- BlockConnectionCommand.cs          (Existing)
|   +-- BlockConnectionRequestHandler.cs   (Updated: enforce Accepted-only, save StatusBeforeBlock, swap CustomerId)
+-- UnblockConnection/
    |-- UnblockConnectionCommand.cs        (New)
    |-- UnblockConnectionHandler.cs        (New)
    +-- UnblockConnectionValidator.cs      (New)
```

### 3.2 Updated Logic: `BlockConnectionRequestHandler`
1. Extract `IdentityUserId` from `request.UserId` (populated via `UserEnrichmentBehavior`).
2. Retrieve current `Customer` record via `ICustomerRepository.GetByIdentityIdToEditAsync`.
3. Validate not self-block (`currentUser.CustomerId != request.TargetCustomerId`).
4. Validate target customer exists via `ICustomerRepository.GetCustomerByIdAsync`.
5. Retrieve connection via `IConnectionRepository.GetConnectionAsync(currentUser.CustomerId, request.TargetCustomerId)`:
   - **If connection is null**: Throw `InvalidOperationException("Cannot block a user who is not an accepted connection.")`.
   - **If connection.Status != ConnectionStatus.Accepted**: Throw `InvalidOperationException("Can only block users with an active Accepted connection.")`.
6. Record previous status: `connection.StatusBeforeBlock = connection.Status;`
7. Set blocker ownership: Ensure `connection.CustomerId = currentUser.CustomerId` and `connection.ConnectedCustomerId = request.TargetCustomerId`.
8. Set status: `connection.Status = ConnectionStatus.Blocked;`
9. Call `_connectionRepository.Update(connection)` and `_unitOfWork.SaveChangesAsync()`.

### 3.3 New Logic: `UnblockConnectionHandler`
1. Extract `IdentityUserId` from `request.UserId`.
2. Retrieve current `Customer` record via `ICustomerRepository.GetByIdentityIdToEditAsync`.
3. Validate target customer exists via `ICustomerRepository.GetCustomerByIdAsync`.
4. Retrieve connection via `IConnectionRepository.GetConnectionAsync(currentUser.CustomerId, request.TargetCustomerId)`:
   - **If connection is null**: Throw `KeyNotFoundException("Connection not found.")`.
   - **If connection.Status != ConnectionStatus.Blocked**: Throw `InvalidOperationException("This connection is not currently blocked.")`.
   - **If connection.CustomerId != currentUser.CustomerId**: Throw `UnauthorizedAccessException("Only the user who initiated the block can unblock.")`.
5. Restore status: `connection.Status = connection.StatusBeforeBlock ?? ConnectionStatus.Accepted;`
6. Clear saved state: `connection.StatusBeforeBlock = null;`
7. Call `_connectionRepository.Update(connection)` and `_unitOfWork.SaveChangesAsync()`.

---

## 4. Security & Validation Boundaries

1. **Authentication**: All endpoints protected with `[Authorize]`.
2. **User Context**: Caller identity extracted strictly from JWT claims via `UserEnrichmentBehavior` (`IUserRequest.UserId`).
3. **Blocker Authorization**: Only the user who blocked (`connection.CustomerId`) can perform the unblock operation. The blocked party (`connection.ConnectedCustomerId`) receives `403 Forbidden` / `UnauthorizedAccessException` if attempting to unblock.
4. **State Machine Invariants**:
   - `Block`: Only valid when current state is `Accepted`. Transition: `Accepted` -> `Blocked` (saves `StatusBeforeBlock = Accepted`).
   - `Unblock`: Only valid when current state is `Blocked`. Transition: `Blocked` -> `Accepted` (clears `StatusBeforeBlock = null`).
5. **Input Validation**: `UnblockConnectionValidator` ensures `TargetCustomerId` is not empty and not equal to caller.
