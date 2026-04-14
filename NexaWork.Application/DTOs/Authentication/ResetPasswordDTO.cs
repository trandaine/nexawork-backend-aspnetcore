using System.ComponentModel.DataAnnotations;

namespace NexaWork.Application.DTOs.Authentication;

public class ResetPasswordDTO
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    public string NewPassword { get; set; } = string.Empty;
}