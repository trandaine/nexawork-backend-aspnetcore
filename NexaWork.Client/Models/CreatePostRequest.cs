using NexaWork.Domain.Enums;

namespace NexaWork.Client.Models;

public class CreatePostRequest
{
    public string Content { get; set; } = string.Empty;
    public IFormFile? MediaFile { get; set; }
    public VisibilityLevel Visibility { get; set; }
}
