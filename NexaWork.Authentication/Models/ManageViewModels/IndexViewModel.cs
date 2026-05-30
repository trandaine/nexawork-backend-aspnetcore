namespace NexaWork.Authentication.Models.ManageViewModels;

public class IndexViewModel
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public string? StatusMessage { get; set; }
}
