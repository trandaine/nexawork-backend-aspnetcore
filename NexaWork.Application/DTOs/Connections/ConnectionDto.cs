using NexaWork.Domain.Enums;

namespace NexaWork.Application.DTOs.Connections;

public class ConnectionDto
{
    public Guid ConnectionId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ConnectedCustomerId { get; set; }
    public ConnectionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // The other user's information
    public Guid TargetUserId { get; set; }
    public string TargetFirstName { get; set; } = string.Empty;
    public string TargetLastName { get; set; } = string.Empty;
    public string? TargetHeadline { get; set; }
    public string? TargetProfilePictureUrl { get; set; }
}
