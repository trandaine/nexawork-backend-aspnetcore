using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace NexaWork.Authentication.Models.ManageViewModels;

public class EnableAuthenticatorViewModel
{
    [Required(ErrorMessage = "The code is required.")]
    [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Text)]
    [Display(Name = "Verification Code")]
    public string Code { get; set; } = string.Empty;

    [ValidateNever]
    public string SharedKey { get; set; } = string.Empty;

    [ValidateNever]
    public string AuthenticatorUri { get; set; } = string.Empty;
    
    [ValidateNever]
    public string QrCodeBase64 { get; set; } = string.Empty;
}
