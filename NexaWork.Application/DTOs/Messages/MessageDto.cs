using System;

namespace NexaWork.Application.DTOs.Messages;

public class MessageDto
{
    public Guid MessageId { get; set; }
    public Guid SenderCustomerId { get; set; }
    public string SenderFirstName { get; set; } = string.Empty;
    public string SenderLastName { get; set; } = string.Empty;
    public string? SenderProfilePictureUrl { get; set; }
    public Guid ReceiverCustomerId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
