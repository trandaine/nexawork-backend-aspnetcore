using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;


namespace NexaWork.Application.DTOs.Authentication;

public class LoginDTO
{
    // public IList<AuthenticationScheme>? ExternalLogins { get; set; }
    // public string? ReturnUrl { get; set; }
    // public string? ErrorMessage { get; set; }


    // [Required]
    // [EmailAddress]
    // public string? Email { get; set; }
    // [Required]
    // [DataType(DataType.Password)]
    // public string Password { get; set; }
    // [Display(Name = "Remember me?")]
    // public bool RememberMe { get; set; }

    [Required(ErrorMessage = "Tên đăng nhập hoặc Email là bắt buộc")]
    public required string UsernameOrEmail { get; set; } = string.Empty;
    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    public required string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; } = false;


    /// <summary>
    /// The optional two-factor authenticator code. This may be required for users who have enabled two-factor authentication.
    /// This is not required if a <see cref="TwoFactorRecoveryCode"/> is sent.
    /// </summary>
    public string? TwoFactorCode { get; set; }

    /// <summary>
    /// An optional two-factor recovery code from <see cref="TwoFactorResponse.RecoveryCodes"/>.
    /// This is required for users who have enabled two-factor authentication but lost access to their <see cref="TwoFactorCode"/>.
    /// </summary>
    public string? TwoFactorRecoveryCode { get; set; }
}

