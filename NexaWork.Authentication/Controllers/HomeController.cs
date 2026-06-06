using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Authentication.Data.IdentityEntities;

namespace NexaWork.Authentication.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly UserManager<NexaWorkUser> _userManager;

    public HomeController(UserManager<NexaWorkUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // We can pass user info to the view to make the dashboard personalized.
        ViewData["FullName"] = user.UserName; // Assuming Username is their primary handle here
        ViewData["Email"] = user.Email;

        return View();
    }
}
