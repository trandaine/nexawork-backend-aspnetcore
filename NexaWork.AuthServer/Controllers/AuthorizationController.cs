using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NexaWork.AuthServer.Data.IdentityEntities;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace NexaWork.AuthServer.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly SignInManager<NexaWorkUser> _signInManager;
        private readonly UserManager<NexaWorkUser> _userManager;

        public AuthorizationController(SignInManager<NexaWorkUser> signInManager, UserManager<NexaWorkUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // The Authorize Endpoint (Redirects to Login, then issues Authorization Code)
        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // Check if the user is logged in via the standard Identity cookie
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);

            if (!result.Succeeded)
            {
                // If not logged in, redirect to AccountController/Login
                return Challenge(
                    authenticationSchemes: IdentityConstants.ApplicationScheme,
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                            Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                    });
            }

            // The user is logged in! Fetch them from the database
            var user = await _userManager.GetUserAsync(result.Principal) ??
                throw new InvalidOperationException("The user details cannot be retrieved.");

            // Create a new ClaimsPrincipal to hand over to OpenIddict
            var principal = await _signInManager.CreateUserPrincipalAsync(user);


            // Explicitly set the core OpenIddict claims
            principal.SetClaim(OpenIddictConstants.Claims.Subject, await _userManager.GetUserIdAsync(user));
            principal.SetClaim(OpenIddictConstants.Claims.Email, await _userManager.GetEmailAsync(user));
            principal.SetClaim(OpenIddictConstants.Claims.Name, await _userManager.GetUserNameAsync(user));
            // Tell OpenIddict what scopes/permissions the token will have
            principal.SetScopes(request.GetScopes());

            principal.SetResources("nexawork_client_api"); // The audience name for your backend API which is used to access the resources.

            // Attach destinations to every claim so OpenIddict knows where to put them
            foreach (var claim in principal.Claims)
            {
                claim.SetDestinations(GetDestinations(claim, principal));
            }

            // Issue the Authorization Code and redirect back to the React app
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        // The Token Endpoint (Exchanges the Code for the JWT)
        [HttpPost("~/connect/token")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
            {
                // Because we already authenticated the user in the Authorize endpoint,
                // the Authorization Code already contains all their claims. 
                // We just extract them and issue the final token.
                var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                // Re-create the principal from the stored code/refresh token
                var principal = result.Principal;

                // Return the JWT Access Token
                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.UnsupportedGrantType,
                ErrorDescription = "The specified grant type is not supported."
            });
        }



        /// <summary>
        /// This method determines which claims go into the Access Token vs the Identity Token.
        /// </summary>
        /// <param name="claim"></param>
        /// <param name="principal"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
        {
            // 1. Never include the Security Stamp in tokens (it is a secret ASP.NET Core Identity value)
            if (claim.Type == "AspNet.Identity.SecurityStamp")
            {
                yield break;
            }

            // 2. All other claims go to the Access Token so your backend API can read them
            yield return OpenIddictConstants.Destinations.AccessToken;

            // 3. Only send standard profile claims to the Identity Token (which is read by the React frontend)
            if (claim.Type == OpenIddictConstants.Claims.Name || claim.Type == ClaimTypes.Name ||
                claim.Type == OpenIddictConstants.Claims.Email || claim.Type == ClaimTypes.Email ||
                claim.Type == OpenIddictConstants.Claims.Subject || claim.Type == ClaimTypes.NameIdentifier)
            {
                yield return OpenIddictConstants.Destinations.IdentityToken;
            }
        }


        /// <summary>
        /// The logout endpoint
        /// This endpoint's only job is to destroy the Identity cookie and tell OpenIddict to perform the final redirect
        /// </summary>
        /// <returns></returns>
        [HttpGet("~/connect/logout")]
        [HttpPost("~/connect/logout")]
        public async Task<IActionResult> Logout()
        {
            // 1. Destroy the ASP.NET Core Identity cookie
            await _signInManager.SignOutAsync();

            // 2. Tell OpenIddict to clean up its own session and redirect the user back 
            // to the "PostLogoutRedirectUri" you configured in the React client's database record.
            return SignOut(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = "/"
                });
        }
    }
}
