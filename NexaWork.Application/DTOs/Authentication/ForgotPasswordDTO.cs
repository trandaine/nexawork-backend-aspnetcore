
using System.ComponentModel.DataAnnotations;

namespace NexaWork.Application.DTOs.Authentication;


public class ForgotPasswordDTO
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
