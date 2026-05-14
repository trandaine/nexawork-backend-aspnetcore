using NexaWork.Domain.Enums;

namespace NexaWork.Client.Models;

public class UpdatePostRequest
{
    public string Content { get; set; } = string.Empty;
    public IFormFile? MediaFile { get; set; } // IFormFile is an ASP.NET Core type, so it belongs in the API layer
    public VisibilityLevel Visibility { get; set; }
}
