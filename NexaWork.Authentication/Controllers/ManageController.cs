using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Authentication.Data.IdentityEntities;
using NexaWork.Authentication.Models.ManageViewModels;
using QRCoder;
using System.Text.Encodings.Web;
using NexaWork.Authentication.Data;

namespace NexaWork.Authentication.Controllers;

[Authorize]
public class ManageController : Controller
{
    private readonly UserManager<NexaWorkUser> _userManager;
    private readonly SignInManager<NexaWorkUser> _signInManager;
    private readonly UrlEncoder _urlEncoder;
    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

    private readonly NexaWorkIdentityDbContext _context;
    private readonly NexaWork.Authentication.Services.IEmailSender _emailSender;

    public ManageController(
        UserManager<NexaWorkUser> userManager,
        SignInManager<NexaWorkUser> signInManager,
        UrlEncoder urlEncoder,
        NexaWorkIdentityDbContext context,
        NexaWork.Authentication.Services.IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _urlEncoder = urlEncoder;
        _context = context;
        _emailSender = emailSender;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var model = new IndexViewModel
        {
            Username = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            IsEmailConfirmed = user.EmailConfirmed
        };
        
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ChangePassword()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var hasPassword = await _userManager.HasPasswordAsync(user);
        if (!hasPassword)
        {
            return RedirectToAction(nameof(SetPassword));
        }

        return View(new ChangePasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        await _signInManager.RefreshSignInAsync(user);
        model.StatusMessage = "Your password has been changed.";
        return View(model);
    }
    
    [HttpGet]
    public IActionResult SetPassword()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> TwoFactorAuthentication()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        ViewData["Is2faEnabled"] = is2faEnabled;
        ViewData["RecoveryCodesLeft"] = await _userManager.CountRecoveryCodesAsync(user);
        ViewData["HasAuthenticator"] = await _userManager.GetAuthenticatorKeyAsync(user) != null;

        var passkeys = _context.FidoStoredCredentials.Where(c => c.UserId == user.Id).ToList();
        ViewData["Passkeys"] = passkeys;
        ViewData["Preferred2faMethod"] = user.Preferred2faMethod;

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> EnableAuthenticator()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var model = new EnableAuthenticatorViewModel();
        await LoadSharedKeyAndQrCodeUriAsync(user, model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        if (!ModelState.IsValid)
        {
            await LoadSharedKeyAndQrCodeUriAsync(user, model);
            return View(model);
        }

        var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
        var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
            user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!is2faTokenValid)
        {
            ModelState.AddModelError("Code", "Verification code is invalid.");
            await LoadSharedKeyAndQrCodeUriAsync(user, model);
            return View(model);
        }

        await _userManager.SetTwoFactorEnabledAsync(user, true);

        if (await _userManager.CountRecoveryCodesAsync(user) == 0)
        {
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            return RedirectToAction(nameof(ShowRecoveryCodes), new { codes = recoveryCodes });
        }

        return RedirectToAction(nameof(TwoFactorAuthentication));
    }

    [HttpGet]
    public IActionResult ShowRecoveryCodes(string[] codes)
    {
        if (codes == null || codes.Length == 0)
        {
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        var model = new ShowRecoveryCodesViewModel { RecoveryCodes = codes };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateRecoveryCodes()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        if (!isTwoFactorEnabled)
        {
            return BadRequest("Cannot generate recovery codes as 2FA is not enabled.");
        }

        var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        return RedirectToAction(nameof(ShowRecoveryCodes), new { codes = recoveryCodes });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Disable2fa()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
        if (!disable2faResult.Succeeded)
        {
            throw new InvalidOperationException($"Unexpected error occurred disabling 2FA for user with ID '{_userManager.GetUserId(User)}'.");
        }

        return RedirectToAction(nameof(TwoFactorAuthentication));
    }

    private async Task LoadSharedKeyAndQrCodeUriAsync(NexaWorkUser user, EnableAuthenticatorViewModel model)
    {
        var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        model.SharedKey = FormatKey(unformattedKey!);
        var email = await _userManager.GetEmailAsync(user);
        model.AuthenticatorUri = GenerateQrCodeUri(email!, unformattedKey!);
        model.QrCodeBase64 = GenerateQrCodeBase64(model.AuthenticatorUri);
    }

    private string FormatKey(string unformattedKey)
    {
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
        return string.Format(
            AuthenticatorUriFormat,
            _urlEncoder.Encode("NexaWork"),
            _urlEncoder.Encode(email),
            unformattedKey);
    }

    private string GenerateQrCodeBase64(string text)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeImage = qrCode.GetGraphic(20);
        return Convert.ToBase64String(qrCodeImage);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPreferred2faMethod(string method)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        user.Preferred2faMethod = string.IsNullOrEmpty(method) ? null : method;
        await _userManager.UpdateAsync(user);

        return RedirectToAction(nameof(TwoFactorAuthentication));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemovePasskey(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var passkey = _context.FidoStoredCredentials.FirstOrDefault(c => c.Id == id && c.UserId == user.Id);
        if (passkey != null)
        {
            _context.FidoStoredCredentials.Remove(passkey);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(TwoFactorAuthentication));
    }

        [HttpGet]
        public async Task<IActionResult> EnableEmail2fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            var emailHtml = NexaWork.Authentication.Services.EmailTemplates.GetVerificationEmailHtml(
                code, "Setup Email 2FA", "You are setting up Email as a Two-Factor Authentication method. Please enter the verification code below to confirm this action.");
            
            await _emailSender.SendEmailAsync(user.Email, "NexaWork Email 2FA Setup", emailHtml);

            ViewData["Email"] = user.Email;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableEmail2fa(string code)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", code);
            if (isValid)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                
                // Auto set user's preferred 2FA method to Email when they enable it for the first time
                // user.Preferred2faMethod = "Email";
                // await _userManager.UpdateAsync(user);

                TempData["StatusMessage"] = "Email 2FA has been verified and enabled.";
                return RedirectToAction(nameof(TwoFactorAuthentication));
            }

            ModelState.AddModelError(string.Empty, "Invalid verification code.");
            ViewData["Email"] = user.Email;
            return View();
    }
}