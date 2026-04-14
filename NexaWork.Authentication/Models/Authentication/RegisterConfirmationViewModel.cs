using System;

namespace NexaWork.Authentication.Models.Authentication;

public class RegisterConfirmationViewModel
{
    public string Email { get; set; }

    public bool DisplayConfirmAccountLink { get; set; }

    public string EmailConfirmationUrl { get; set; }
    public string returnUrl = null;
}
