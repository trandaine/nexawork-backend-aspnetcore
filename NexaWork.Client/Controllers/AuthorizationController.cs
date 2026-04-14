using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NexaWork.Domain.IdentityEntites;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace NexaWork.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserManager<NexaWorkUser> _userManager;
        private readonly SignInManager<NexaWorkUser> _signInManager; // THÊM SignInManager

        // Cập nhật Constructor để Inject SignInManager
        public AuthorizationController(
            UserManager<NexaWorkUser> userManager,
            SignInManager<NexaWorkUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Endpoint 1: Xử lý quy trình Authorization Code
        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            var authenticateResult = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);

            if (!authenticateResult.Succeeded)
            {
                // SỬA LỖI 1: Bỏ named parameters, truyền trực tiếp
                return Challenge(
                    new AuthenticationProperties
                    {
                        RedirectUri = Request.PathBase + Request.Path + Request.QueryString
                    },
                    IdentityConstants.ApplicationScheme);
            }

            var user = await _userManager.GetUserAsync(authenticateResult.Principal);

            // SỬA LỖI 2: Dùng SignInManager để tạo ra ClaimsPrincipal chuẩn
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            principal.SetScopes(request.GetScopes());
            principal.SetDestinations(GetDestinations);

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }



        // Endpoint 2: Đổi mã Code lấy JWT (JSON Web Token)
        [HttpPost("~/connect/token")]
        [Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            if (request.IsAuthorizationCodeGrantType())
            {
                var authenticateResult = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                return SignIn(authenticateResult.Principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            throw new InvalidOperationException("The specified grant type is not supported.");
        }

        private static IEnumerable<string> GetDestinations(Claim claim)
        {
            yield return Destinations.AccessToken;

            if (claim.Type == Claims.Name || claim.Type == Claims.Email || claim.Type == Claims.Role)
                yield return Destinations.IdentityToken;
        }
    }
}
