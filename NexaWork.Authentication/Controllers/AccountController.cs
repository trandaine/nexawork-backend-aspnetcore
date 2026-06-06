using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Authentication.Data.IdentityEntities;
using MassTransit;
using NexaWork.Authentication.Models.AccountViewModels;
using NexaWork.Contracts;

namespace NexaWork.Authentication.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly SignInManager<NexaWorkUser> _signInManager;
        private readonly UserManager<NexaWorkUser> _userManager;
        private readonly NexaWork.Authentication.Services.IEmailSender _emailSender;
        private readonly NexaWork.Authentication.Data.NexaWorkIdentityDbContext _context;

        public AccountController(
            SignInManager<NexaWorkUser> signInManager,
            UserManager<NexaWorkUser> userManager,
            IPublishEndpoint publishEndpoint,
            NexaWork.Authentication.Services.IEmailSender emailSender,
            NexaWork.Authentication.Data.NexaWorkIdentityDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _publishEndpoint = publishEndpoint;
            _emailSender = emailSender;
            _context = context;
        }

        [HttpGet("Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var user = await _userManager.FindByEmailAsync(model.Email)
                       ?? await _userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Username or Password not match.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    var cleanReturnUrl = returnUrl.Split(',')[0];
                    if (Url.IsLocalUrl(cleanReturnUrl))
                    {
                        return Redirect(cleanReturnUrl);
                    }
                }

                return Redirect("~/");
            }

            if (result.RequiresTwoFactor)
            {
                if (user.Preferred2faMethod == "Passkey")
                {
                    return RedirectToAction(nameof(LoginWithPasskey), new { returnUrl });
                }
                else if (user.Preferred2faMethod == "TOTP")
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl });
                }
                else if (user.Preferred2faMethod == "Email")
                {
                    return RedirectToAction(nameof(LoginWithEmailCode), new { rememberMe = false, returnUrl });
                }

                return RedirectToAction(nameof(Select2faProvider), new { returnUrl });
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account locked out.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "(401) Username or Password not match.");
            return View(model);
        }

        [HttpGet("LoginWith2fa")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string? returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost("LoginWith2fa")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            var result =
                await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, model.RememberMe,
                    model.RememberMachine);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    var cleanReturnUrl = returnUrl.Split(',')[0];
                    if (Url.IsLocalUrl(cleanReturnUrl))
                    {
                        return Redirect(cleanReturnUrl);
                    }
                }

                return Redirect("~/");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account locked out.");
                return View(model);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View(model);
            }
        }

        [HttpGet("LoginWithRecoveryCode")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string? returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("LoginWithRecoveryCode")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model,
            string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);
            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    var cleanReturnUrl = returnUrl.Split(',')[0];
                    if (Url.IsLocalUrl(cleanReturnUrl))
                    {
                        return Redirect(cleanReturnUrl);
                    }
                }

                return Redirect("~/");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account locked out.");
                return View(model);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View(model);
            }
        }

        [HttpGet("Select2faProvider")]
        [AllowAnonymous]
        public async Task<IActionResult> Select2faProvider(string? returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException("Unable to load two-factor authentication user.");
            }

            ViewData["HasEmail"] = await _userManager.GetTwoFactorEnabledAsync(user);
            ViewData["HasAuthenticator"] = await _userManager.GetAuthenticatorKeyAsync(user) != null;
            ViewData["HasPasskeys"] = _context.FidoStoredCredentials.Any(c => c.UserId == user.Id);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("Select2faProvider")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Select2faProvider(string provider, bool rememberChoice,
            string? returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null) return RedirectToAction("Login");

            if (rememberChoice)
            {
                user.Preferred2faMethod = provider;
                await _userManager.UpdateAsync(user);
            }

            if (provider == "Passkey")
            {
                return RedirectToAction(nameof(LoginWithPasskey), new { returnUrl });
            }
            else if (provider == "Email")
            {
                return RedirectToAction(nameof(LoginWithEmailCode), new { rememberMe = false, returnUrl });
            }

            return RedirectToAction(nameof(LoginWith2fa), new { returnUrl });
        }

        [HttpGet("LoginWithPasskey")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithPasskey(string? returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null) return RedirectToAction("Login");

            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Username"] = user.UserName;
            return View();
        }

        [HttpPost("LoginCallbackWithPasskey")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginCallbackWithPasskey(string? returnUrl = null)
        {
            var userId = HttpContext.Session.GetString("fido2.authenticatedUserId");
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError(string.Empty, "Session expired or invalid FIDO2 login.");
                return View("Login");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return RedirectToAction("Login");

            await _signInManager.SignInAsync(user, isPersistent: false);
            HttpContext.Session.Remove("fido2.authenticatedUserId");

            if (!string.IsNullOrEmpty(returnUrl))
            {
                var cleanReturnUrl = returnUrl.Split(',')[0];
                if (Url.IsLocalUrl(cleanReturnUrl))
                {
                    return Redirect(cleanReturnUrl);
                }
            }

            return Redirect("~/");
        }

        [HttpGet("Register")]
        [AllowAnonymous]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterRequestDTO model, string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(returnUrl)) returnUrl = returnUrl.Split(',')[0];
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = new NexaWorkUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _publishEndpoint.Publish(new UserRegisteredEvent
                    {
                        UserId = user.Id,
                        Email = user.Email
                    });

                    var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                    var emailHtml = NexaWork.Authentication.Services.EmailTemplates.GetVerificationEmailHtml(
                        code, "Welcome to NexaWork!",
                        "Thanks for creating an account with NexaWork. We are excited to have you on board! This is your first time you log in to NexaWork. We hope you have a great experience using our platform. Please enter the verification code below to verify your email address and complete your account setup.");

                    await _emailSender.SendEmailAsync(user.Email, "Verify your NexaWork account", emailHtml);

                    return RedirectToAction(nameof(VerifyEmail), new { email = user.Email, returnUrl });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }

        [HttpGet("VerifyEmail")]
        [AllowAnonymous]
        public IActionResult VerifyEmail(string email, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Email"] = email;
            return View();
        }

        [HttpPost("VerifyEmail")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(string email, string code, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Email"] = email;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View();
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", code);
            if (isValid)
            {
                user.EmailConfirmed = true;
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                user.Preferred2faMethod = "Email";
                await _userManager.UpdateAsync(user);
                await _signInManager.SignInAsync(user, isPersistent: false);
                
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    var cleanReturnUrl = returnUrl.Split(',')[0];
                    if (Url.IsLocalUrl(cleanReturnUrl))
                    {
                        return Redirect(cleanReturnUrl);
                    }
                }

                return Redirect("~/");
            }

            ModelState.AddModelError(string.Empty, "Invalid verification code.");
            return View();
        }

        [HttpGet("LoginWithEmailCode")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithEmailCode(bool rememberMe, string? returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            var emailHtml = NexaWork.Authentication.Services.EmailTemplates.GetVerificationEmailHtml(
                code, "Your NexaWork Login Code",
                "You are trying to log in to NexaWork. Please enter the verification code below to complete your sign in.");

            await _emailSender.SendEmailAsync(user.Email, "Your Login Code", emailHtml);

            return View(new LoginWith2faViewModel { RememberMe = rememberMe });
        }

        [HttpPost("LoginWithEmailCode")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithEmailCode(LoginWith2faViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.TwoFactorSignInAsync("Email", model.TwoFactorCode, model.RememberMe,
                rememberClient: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    var cleanReturnUrl = returnUrl.Split(',')[0];
                    if (Url.IsLocalUrl(cleanReturnUrl))
                    {
                        return Redirect(cleanReturnUrl);
                    }
                }
                return Redirect("~/");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "User account locked out.");
                return View(model);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login code.");
                return View(model);
            }
        }
        [HttpGet("ForgotPassword")]
        [AllowAnonymous]
        public IActionResult ForgotPassword(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(returnUrl)) returnUrl = returnUrl.Split(',')[0];
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(VerifyResetCode), new { email = model.Email, returnUrl });
                }

                // Generate a 6-digit code for password reset using the Email token provider
                var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                
                var emailHtml = NexaWork.Authentication.Services.EmailTemplates.GetVerificationEmailHtml(
                    code, "Reset your NexaWork password",
                    "We received a request to reset your password. Please enter the verification code below to proceed with resetting your password. If you did not request a password reset, you can safely ignore this email.");

                await _emailSender.SendEmailAsync(model.Email, "Reset your password", emailHtml);

                return RedirectToAction(nameof(VerifyResetCode), new { email = model.Email, returnUrl });
            }

            return View(model);
        }

        [HttpGet("VerifyResetCode")]
        [AllowAnonymous]
        public IActionResult VerifyResetCode(string email, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Email"] = email;
            
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction(nameof(ForgotPassword), new { returnUrl });
            }
            
            return View();
        }

        [HttpPost("VerifyResetCode")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyResetCode(VerifyResetCodeViewModel model, string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(returnUrl)) returnUrl = returnUrl.Split(',')[0];
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Email"] = model.Email;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal user doesn't exist
                ModelState.AddModelError(string.Empty, "Invalid verification code.");
                return View(model);
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", model.Code);
            if (isValid)
            {
                // Generate the highly secure Identity reset token in the background
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                
                // Redirect to the ResetPassword page passing the token
                return RedirectToAction(nameof(ResetPassword), new { email = model.Email, resetToken, returnUrl });
            }

            ModelState.AddModelError(string.Empty, "Invalid verification code.");
            return View(model);
        }

        [HttpGet("ResetPassword")]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string resetToken, string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(resetToken))
            {
                return BadRequest("A valid email and token must be supplied for password reset.");
            }

            ViewData["ReturnUrl"] = returnUrl;
            
            var model = new ResetPasswordViewModel
            {
                Email = email,
                ResetToken = resetToken
            };
            
            return View(model);
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(returnUrl)) returnUrl = returnUrl.Split(',')[0];
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(Login), new { returnUrl });
            }

            var result = await _userManager.ResetPasswordAsync(user, model.ResetToken, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Your password has been reset successfully. Please log in with your new password.";
                return RedirectToAction(nameof(Login), new { returnUrl });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}