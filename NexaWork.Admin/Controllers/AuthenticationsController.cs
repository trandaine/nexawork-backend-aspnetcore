using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NexaWork.Domain.IdentityEntites;
using NexaWork.Admin.Models;

namespace NexaWork.Admin.Controllers;

public class AuthenticationsController : Controller
{
    private readonly SignInManager<NexaWorkUser> _signInManager;
    private readonly UserManager<NexaWorkUser> _userManager;
    private readonly IUserStore<NexaWorkUser> _userStore;
    private readonly IUserEmailStore<NexaWorkUser> _emailStore;
    private readonly ILogger<RegisterViewModel> _logger;
    // private readonly IEmailSender _emailSender;
    public AuthenticationsController(SignInManager<NexaWorkUser> signInManager,
    UserManager<NexaWorkUser> userManager,
    IUserStore<NexaWorkUser> userStore,
    // IUserEmailStore<NexaWorkUser> emailStore,
    ILogger<RegisterViewModel> logger
    // IEmailSender emailSender
    )
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userStore = userStore;
        // _emailStore = emailStore;
        _emailStore = GetEmailStore();

        _logger = logger;
        // _emailSender = emailSender;
    }



    public IActionResult Register(string? returnUrl = null)
    {
        var registerViewModel = new RegisterViewModel
        {
            ReturnUrl = returnUrl,

        };
        // ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        return View(registerViewModel);
    }


    // ----------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Hàm tạo tài khoản mới cho người dùng. Sau khi nhận dữ liệu sẽ chuyển sang trang RegisterConfirmation
    /// </summary>
    /// <param name="registerViewModel"></param>
    /// <returns></returns>
    // [HttpPost]
    // public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    // {
    //     try
    //     {
    //         var returnUrl = string.IsNullOrEmpty(registerViewModel.ReturnUrl) ? Url.Content("~/") : registerViewModel.ReturnUrl;
    //         // var ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    //         if (ModelState.IsValid)
    //         {
    //             var user = CreateUser();

    //             await _userStore.SetUserNameAsync(user, registerViewModel.Email, CancellationToken.None);
    //             await _emailStore.SetEmailAsync(user, registerViewModel.Email, CancellationToken.None);
    //             var result = await _userManager.CreateAsync(user, registerViewModel.Password);

    //             if (result.Succeeded)
    //             {
    //                 _logger.LogInformation("User created a new account with password.");

    //                 var userId = await _userManager.GetUserIdAsync(user);
    //                 var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
    //                 code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

    //                 // Hàm tạo link hoàn chỉnh để xác nhận email. Hiện tại hàm này chỉ sử dụng với công nghệ Razor Pages. Logic hoàn chỉnh ở dứoi hàm RegisterConfirmation
    //                 // Generate the Admin URL to the email sender page which will trigger the email sending logic
    //                 // var callbackUrl = Url.Page(
    //                 //     "/Account/ConfirmEmail",
    //                 //     pageHandler: null,
    //                 //     values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
    //                 //     protocol: Request.Scheme);

    //                 // await _emailSender.SendEmailAsync(registerViewModel.Email, "Confirm your email",
    //                 //     $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

    //                 if (_userManager.Options.SignIn.RequireConfirmedAccount)
    //                 {
    //                     return RedirectToAction("RegisterConfirmation", new { email = registerViewModel.Email, returnUrl = returnUrl });
    //                 }
    //                 else
    //                 {
    //                     await _signInManager.SignInAsync(user, isPersistent: false);
    //                     return LocalRedirect(returnUrl);
    //                 }
    //             }
    //             foreach (var error in result.Errors)
    //             {
    //                 ModelState.AddModelError(string.Empty, error.Description);
    //             }
    //             return RedirectToAction(nameof(Login));

    //         }
    //     }
    //     catch (System.Exception ex)
    //     {
    //         _logger.LogError("Error in registration process: {Message}", ex.Message);
    //         throw;
    //     }


    //     // If we got this far, something failed, redisplay form
    //     return View(registerViewModel);
    // }
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken] // Important for security on POST requests
    public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        // 1. Validate the form data (checks if required fields are filled, passwords match, etc.)
        if (ModelState.IsValid)
        {
            try
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);

                // 2. Attempt to create the user in the database
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // 3. Generate the Email Confirmation Token
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    // 4. Generate the Admin-friendly confirmation link
                    // This targets the ConfirmEmail action we created earlier
                    var callbackUrl = Url.Action(
                        action: "ConfirmEmail",
                        controller: "Authentications", // Replace "Auth" with your actual controller name
                        values: new { userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    // 5. TODO: Implement your email sending service here
                    // await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                    //     $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    // 6. Handle application flow based on confirmation requirements
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        // Redirects to the RegisterConfirmation GET method we wrote previously
                        return RedirectToAction("RegisterConfirmation", new { email = model.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        // If confirmation isn't strictly required, log them in immediately
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }

                // 7. Handle Registration Errors (e.g., Email already taken, password too weak)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                // NOTE: I removed the `return RedirectToAction(nameof(Login));` that was here.
                // If creation fails, we must fall through to `return View(model)` below 
                // so the user actually sees the validation errors on the form!
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Error in registration process: {Message}", ex.Message);
                ModelState.AddModelError(string.Empty, "An unexpected error occurred during registration. Please try again.");
            }
        }

        // 8. If we got this far, something failed (invalid form or failed creation). Redisplay the form with errors.
        return View(model);
    }

    private NexaWorkUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<NexaWorkUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(NexaWorkUser)}'. " +
                $"Ensure that '{nameof(NexaWorkUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }

    private IUserEmailStore<NexaWorkUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<NexaWorkUser>)_userStore;
    }





    // ------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Action này xử lý việc hiển thị trang xác nhận đăng ký sau khi người dùng hoàn tất quá trình đăng ký. 
    /// Nó tạo một token xác nhận email và xây dựng một URL để người dùng có thể nhấp vào để xác nhận email của họ. 
    /// Hiện tại, nó chỉ hiển thị liên kết xác nhận trên trang.
    /// TODO: Nên tích hợp logic gửi mail sau này.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterConfirmation(string email, string returnUrl = null)
    {
        // Basic validation
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Index", "Home");
        }

        // Fetch the user
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return NotFound($"Unable to load user with email '{email}'.");
        }

        // Generate and encode the confirmation token
        var userId = await _userManager.GetUserIdAsync(user);
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        // Generate the proper Admin URL 
        // This looks for an action named "ConfirmEmail" inside this same controller
        var confirmationUrl = Url.Action(
            action: "ConfirmEmail",
            controller: "Authentications",
            values: new { userId = userId, code = code, returnUrl = returnUrl },
            protocol: Request.Scheme);

        // TODO: Integrate actual Email Sending logic here
        // await _emailSender.SendEmailAsync(email, "Confirm your email", $"Please confirm your account by clicking this link: {confirmationUrl}");

        // Set up the View Model (Useful for local testing before you set up SMTP)
        var viewModel = new RegisterConfirmationViewModel
        {
            Email = email,
            DisplayConfirmAccountLink = true, // Set to false once you have a real email sender
            EmailConfirmationUrl = confirmationUrl
        };

        // Pass the model to the view so it can be rendered
        return View(viewModel);
    }
    [HttpPost]
    public IActionResult RegisterConfirmation(RegisterConfirmationViewModel registerConfirmationViewModel)
    {
        // This POST action can be used to trigger the email sending logic once you have it set up
        // For now, it just redirects back to the GET action which will display the confirmation link
        return View(registerConfirmationViewModel);
    }






    // ----------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Hàm này sẽ hiển thị ra trang để người dùng xác nhận email sau khi họ nhấp vào liên kết được tạo ra từ hàm RegisterConfirmation.
    /// Hiện tại chỉ có hiển thị sang một trang mới. Nên tích hợp logic xác nhận email sau này.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        // 1. Validate that the required parameters are present in the URL
        if (userId == null || code == null)
        {
            return RedirectToAction("Index", "Home");
        }

        // 2. Find the user in the database
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userId}'.");
        }

        // 3. Decode the token (since we encoded it for safe URL transport earlier)
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

        // 4. Attempt to confirm the email using Identity's built-in method
        var result = await _userManager.ConfirmEmailAsync(user, code);

        // 5. Handle the result and pass a message to the View
        if (result.Succeeded)
        {
            ViewBag.Message = "Thank you for confirming your email. You can now log in.";
            return View(); // This returns Views/Auth/ConfirmEmail.cshtml
        }

        ViewBag.Message = "There was an error confirming your email. The link may be invalid or expired.";
        return View();
    }







    public IActionResult Login(string? returnUrl = null)
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
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
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


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(string? returnUrl = null)
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");
        if (returnUrl != null)
        {
            return LocalRedirect(returnUrl);
        }
        else
        {
            // This needs to be a redirect so that the browser performs a new
            // request and the identity for the user gets updated.
            return RedirectToAction("Index", "Home");
        }
        // return View();
    }









    // ----------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Hàm xử lý đăng nhập với 2 bước xác minh. Người dùng sẽ được chuyển đến trang này sau khi nhập đúng email và mật khẩu nếu tài khoản của họ yêu cầu xác thực hai yếu tố.
    /// </summary>
    /// <param name="rememberMe"></param>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
    {
        // Ensure the user has actually gone through the username/password screen first
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

        if (user == null)
        {
            throw new InvalidOperationException($"Unable to load two-factor authentication user.");
        }

        var model = new LoginWith2faViewModel
        {
            ReturnUrl = returnUrl,
            RememberMe = rememberMe
        };

        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model)
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

        // Strip spaces and hyphens from the code just in case the user typed them
        var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

        // Verify the code
        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
            authenticatorCode,
            model.RememberMe,
            model.RememberMachine);

        if (result.Succeeded)
        {
            _logger.LogInformation("User logged in with 2fa.");

            // Open Redirect Protection
            var returnUrl = model.ReturnUrl ?? Url.Content("~/");
            if (Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        else if (result.IsLockedOut)
        {
            _logger.LogWarning("User account locked out.");
            return RedirectToAction("Lockout");
        }
        else
        {
            _logger.LogWarning("Invalid authenticator code entered.");
            ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
            return View(model);
        }
    }


}
