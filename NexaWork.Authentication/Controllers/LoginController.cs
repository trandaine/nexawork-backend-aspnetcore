using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Application.DTOs.Authentication;
using NexaWork.Authentication.Models;
using NexaWork.Domain.IdentityEntites;

namespace NexaWork.Authentication.Controllers.Authentication;

public class LoginController : Controller
{
    private readonly SignInManager<NexaWorkUser> _signInManager;
    private readonly ILogger<LoginDTO> _logger;

    public LoginController(
        SignInManager<NexaWorkUser> signInManager,
        ILogger<LoginDTO> logger
    )
    {
        _logger = logger;
        _signInManager = signInManager;
    }

    // GET: LoginController
    public IActionResult Index(string? returnUrl = null)
    {
        var loginViewModel = new LoginViewModel
        {
            ReturnUrl = returnUrl
        };
        return View(loginViewModel);
    }


    // ----------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Hàm này xử lý logic đăng nhập cho người dùng. 
    /// </summary>
    /// <param name="loginViewModel"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(LoginViewModel loginViewModel)
    {
        try
        {
            // Safely handle the return URL
            var returnUrl = loginViewModel.ReturnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // Note: lockoutOnFailure is false here. If you want brute-force protection later, set to true.
                var result = await _signInManager.PasswordSignInAsync(
                    loginViewModel.Email,
                    loginViewModel.Password,
                    loginViewModel.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    // Open Redirect Protection (Same as the Register method!)
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction("LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = loginViewModel.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction("Lockout");
                }
                else
                {
                    // This handles wrong passwords or unconfirmed emails (if RequireConfirmedAccount = true)
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(loginViewModel);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Login error: {Message}", ex.Message);
            ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
        }

        // If we got this far, something failed, redisplay form with errors
        return View(loginViewModel);
    }

}
