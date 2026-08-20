# Specification: Real-Time 1-on-1 Messaging (`realtime-messaging`)

- **Change ID**: `realtime-messaging`
- **Status**: Draft / Proposed
- **Author**: Backend Team
- **Created Date**: 2026-08-20

---

## 1. Domain Specification: `Message`

### Entity Definition
- **Class**: `NexaWork.Domain.Entities.Message`
- **Database**: `NexaMessageDB`
- **Table Name**: `Messages`

### Attributes & Rules
```csharp
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

### Constraints & Invariants
1. `MessageId`: Primary key (UUID).
2. `SenderCustomerId` & `ReceiverCustomerId`: Required GUIDs. `SenderCustomerId != ReceiverCustomerId`.
3. `Content`: Required, 1 to 2000 UTF-8 characters.
4. `IsRead`: Boolean, default `false`. When marked read: `IsRead = true`, `ReadAt = DateTime.UtcNow`.
5. `IsDeleted`: Boolean, default `false`. When soft-deleted: `IsDeleted = true`, `DeletedAt = DateTime.UtcNow`.
6. Soft-deleted messages are filtered out globally in `MessageDbContext`.

---

## 2. API Contract Specification

Base path: `/api/messages`  
Header: `Authorization: Bearer <JWT>`

### 2.1 Send Message
- **Method**: `POST`
- **Path**: `/api/messages`
- **Request Body**:
  ```json
  {
    "receiverCustomerId": "11111111-1111-1111-1111-111111111111",
    "content": "Hello there!"
  }
  ```
- **Responses**:
  - `200 OK` / `201 Created`: `{ "messageId": "guid" }`
  - `400 Bad Request`: Validation errors (empty content, self-message, invalid GUID).
  - `403 Forbidden`: Sender and receiver are not connected with `ConnectionStatus.Accepted`, or connection is `Blocked`.
  - `404 Not Found`: Receiver customer not found.

### 2.2 Get Conversation
- **Method**: `GET`
- **Path**: `/api/messages/{otherCustomerId}?page=1&pageSize=20`
- **Responses**:
  - `200 OK`:
    ```json
    [
      {
        "messageId": "22222222-2222-2222-2222-222222222222",
        "senderCustomerId": "11111111-1111-1111-1111-111111111111",
        "senderFirstName": "John",
        "senderLastName": "Doe",
        "senderProfilePictureUrl": "https://...",
        "content": "Hello there!",
        "isRead": true,
        "readAt": "2026-08-20T10:30:00Z",
        "createdAt": "2026-08-20T10:25:00Z"
      }
    ]
    ```

### 2.3 Mark Message as Read
- **Method**: `PUT`
- **Path**: `/api/messages/{messageId}/read`
- **Rules**: Caller must be `ReceiverCustomerId`.
- **Responses**:
  - `204 NoContent`: Success.
  - `403 Forbidden`: Caller is not recipient.
  - `404 Not Found`: Message does not exist.

### 2.4 Mark Conversation as Read
- **Method**: `PUT`
- **Path**: `/api/messages/{otherCustomerId}/read-all`
- **Responses**:
  - `204 NoContent`: Success. All unread messages sent by `otherCustomerId` to caller marked as read.

### 2.5 Delete Message
- **Method**: `DELETE`
- **Path**: `/api/messages/{messageId}`
- **Rules**: Caller must be `SenderCustomerId`.
- **Responses**:
  - `204 NoContent`: Success (soft delete).
  - `403 Forbidden`: Caller is not sender.
  - `404 Not Found`: Message does not exist or already deleted.

---

## 3. Real-Time SignalR Events Contract

Endpoint: `/hubs/chat`

### Client-Received Events:
1. `ReceiveMessage`
   ```json
   {
     "messageId": "guid",
     "senderCustomerId": "guid",
     "senderFirstName": "John",
     "senderLastName": "Doe",
     "senderProfilePictureUrl": "https://...",
     "content": "Hello!",
     "isRead": false,
     "createdAt": "2026-08-20T10:00:00Z"
   }
   ```
2. `MessageRead`
   ```json
   {
     "messageId": "guid",
     "readAt": "2026-08-20T10:05:00Z"
   }
   ```
3. `MessageDeleted`
   ```json
   {
     "messageId": "guid"
   }
   ```
