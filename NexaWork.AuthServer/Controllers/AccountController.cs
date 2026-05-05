using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NexaWork.AuthServer.Data.IdentityEntities;

namespace NexaWork.AuthServer.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {

        private readonly SignInManager<NexaWorkUser> _signInManager;
        private readonly UserManager<NexaWorkUser> _userManager;

        public AccountController(SignInManager<NexaWorkUser> signInManager, UserManager<NexaWorkUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet("Login")]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(); // You will need to create a Views/Account/Login.cshtml file
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // If login is successful, redirect back to the OpenIddict authorization flow
                return LocalRedirect(returnUrl ?? "/");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
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