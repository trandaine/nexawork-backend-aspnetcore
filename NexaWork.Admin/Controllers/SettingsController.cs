using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Admin.Models;
using System.Text.Encodings.Web;

namespace NexaWork.Admin.Controllers;

[Authorize]
public class SettingsController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<SettingsController> _logger;
    public SettingsController(
        UserManager<IdentityUser> userManager,
        ILogger<SettingsController> logger
        )
    {
        _userManager = userManager;
        _logger = logger;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> EnableAuthenticator()
    {
        // 1. Get the currently logged-in user
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound("Unable to load user.");
        }

        // 2. Load the authenticator key from the database (or generate a new one)
        var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        // 3. Format the data for the View
        var email = await _userManager.GetEmailAsync(user);

        var model = new EnableAuthenticatorViewModel
        {
            SharedKey = FormatKey(unformattedKey),
            AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound("Unable to load user.");

        if (!ModelState.IsValid)
        {
            await LoadSharedKeyAndQrCodeUriAsync(user, model);
            return View(model);
        }

        // 1. Strip spaces and hyphens from the submitted code
        var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

        // 2. Verify the code against the Identity system
        var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
            user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!is2faTokenValid)
        {
            ModelState.AddModelError("Code", "Verification code is invalid.");
            await LoadSharedKeyAndQrCodeUriAsync(user, model);
            return View(model);
        }

        // 3. If valid, officially enable 2FA on their account!
        await _userManager.SetTwoFactorEnabledAsync(user, true);
        _logger.LogInformation("User has enabled 2FA with an authenticator app.");

        // Redirect to a success page or home
        TempData["StatusMessage"] = "Your authenticator app has been verified and 2FA is enabled.";
        return RedirectToAction("Index", "Home");
    }


    private async Task LoadSharedKeyAndQrCodeUriAsync(IdentityUser user, EnableAuthenticatorViewModel model)
    {
        var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        model.SharedKey = FormatKey(unformattedKey);
        model.AuthenticatorUri = GenerateQrCodeUri(await _userManager.GetEmailAsync(user), unformattedKey);
    }



    private string FormatKey(string unformattedKey)
    {
        // Inserts a space every 4 characters to make it easier for humans to read
        var result = new System.Text.StringBuilder();
        int currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }
        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition));
        }
        return result.ToString().ToLowerInvariant();
    }

    private string GenerateQrCodeUri(string email, string unformattedKey)
    {
        const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        // Replace "MyPortfolio" with the actual name of your application!
        return string.Format(
            AuthenticatorUriFormat,
            UrlEncoder.Default.Encode("NexaWork"),
            UrlEncoder.Default.Encode(email),
            unformattedKey);
    }
}
