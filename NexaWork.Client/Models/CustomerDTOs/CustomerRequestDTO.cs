namespace NexaWork.Client.Models;

public class CustomerRequestDTO
{
    // public Guid CustomerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Headline { get; set; }
    public string? Summary { get; set; }
    public string? Location { get; set; }
    public IFormFile? ProfilePictureFile { get; set; }
    public IFormFile? BackgroundPictureFile { get; set; }
}