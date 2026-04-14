using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NexaWork.Authentication.Models.Authentication;
using NexaWork.Domain.IdentityEntites;

namespace NexaWork.Authentication.Controllers.Authentication
{
    public class RegisterController : Controller
    {
        private readonly UserManager<NexaWorkUser> _userManager;
        private readonly IUserStore<NexaWorkUser> _userStore;
        private readonly IUserEmailStore<NexaWorkUser> _emailStore;
        private readonly SignInManager<NexaWorkUser> _signInManager;
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(
            UserManager<NexaWorkUser> userManager,
            IUserStore<NexaWorkUser> userStore,
            SignInManager<NexaWorkUser> signInManager,
            ILogger<RegisterController> logger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
        }



    public IActionResult Index(string? returnUrl = null)
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
    //                 // Generate the MVC URL to the email sender page which will trigger the email sending logic
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
    public async Task<IActionResult> Index(RegisterViewModel model, string returnUrl = null)
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

                    // 4. Generate the MVC-friendly confirmation link
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

        // Generate the proper MVC URL 
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


    }
}
