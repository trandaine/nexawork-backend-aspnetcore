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

        public AccountController(
            SignInManager<NexaWorkUser> signInManager,
            UserManager<NexaWorkUser> userManager,
            IPublishEndpoint publishEndpoint)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _publishEndpoint = publishEndpoint;
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

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return Redirect("~/");
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(LoginWith2fa), new { returnUrl });
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
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, model.RememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
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
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string? returnUrl = null)
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
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
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

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl ?? "/");
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
    }
}