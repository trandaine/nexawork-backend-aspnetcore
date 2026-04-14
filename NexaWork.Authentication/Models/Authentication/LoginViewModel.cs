using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace NexaWork.Authentication.Models;

public class LoginViewModel
{
    public IList<AuthenticationScheme>? ExternalLogins { get; set; }
    public string? ReturnUrl { get; set; }
    public string? ErrorMessage { get; set; }


    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}
