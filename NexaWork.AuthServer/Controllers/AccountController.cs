using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NexaWork.AuthServer.Data.IdentityEntities;
using NexaWork.AuthServer.Models;
using MassTransit;
using NexaWork.Contracts;

namespace NexaWork.AuthServer.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly SignInManager<NexaWorkUser> _signInManager;
        private readonly UserManager<NexaWorkUser> _userManager;
        // private readonly ISender _mediator;

        public AccountController(
            SignInManager<NexaWorkUser> signInManager,
            UserManager<NexaWorkUser> userManager,
            // ISender mediator,
            IPublishEndpoint publishEndpoint)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            // _mediator = mediator;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet("Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(); // You will need to create a Views/Account/Login.cshtml file
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
                // Trả về lỗi chung chung để bảo mật
                ModelState.AddModelError(string.Empty, "Username or Password not match.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // // Search for an existing customer linked to this IdentityUser
                // var customerExists = await _mediator.Send(new GetCustomerByIdentityIdQuery(user.Id));
                // if (customerExists == null)
                // {
                //     // If no customer exists, create a new one
                //     var isCustomerCreated = await CreateNewCustomer(user.Id);
                //     if (!isCustomerCreated)
                //     {
                //         // If customer creation fails, log out the user and show an error
                //         await _signInManager.SignOutAsync();
                //         ModelState.AddModelError(string.Empty, "An error occurred while creating your customer profile. Please try again.");
                //         return View(model);
                //     }
                // }
                // // If login is successful, redirect back to the OpenIddict authorization flow
                // return LocalRedirect(returnUrl ?? "/");

                // Security Fix: Prevent Open Redirect Vulnerabilities
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return Redirect("~/");
            }

            ModelState.AddModelError(string.Empty, "(401) Username or Password not match.");
            return View(model);
        }
        // public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        // {
        //     ViewData["ReturnUrl"] = returnUrl;

        //     var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

        //     if (result.Succeeded)
        //     {
        //         // If login is successful, redirect back to the OpenIddict authorization flow
        //         return LocalRedirect(returnUrl ?? "/");
        //     }

        //     ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //     return View();
        // }

        /// <summary>
        /// Method to create a new customer in the system after successful registration. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        // private async Task<bool> CreateNewCustomer(string userId)
        // {
        //     bool isOk = false;
        //     try
        //     {
        //         var command = new CreateCustomerCommand(userId);
        //         await _mediator.Send(command);
        //         isOk = true;
        //     }
        //     catch (System.Exception)
        //     {
        //         // Log the exception (not implemented here for brevity)
        //         isOk = false;
        //     }

        //     return isOk;
        // }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string? returnUrl = null)
        {
            // Pass the ReturnUrl to the view so we don't lose it!
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
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

                // If it failed (e.g., weak password), show errors on the form
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



        // [HttpPost("Logout")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Logout()
        // {
        //     await _signInManager.SignOutAsync();
        //     return Redirect("~/");
        // }

    }
}