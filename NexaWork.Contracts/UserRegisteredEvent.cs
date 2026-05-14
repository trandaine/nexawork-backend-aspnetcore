
namespace NexaWork.Contracts;

public record UserRegisteredEvent
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
