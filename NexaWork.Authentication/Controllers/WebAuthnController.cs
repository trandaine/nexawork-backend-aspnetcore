using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexaWork.Authentication.Data;
using NexaWork.Authentication.Data.IdentityEntities;
using System.Text.Json;

namespace NexaWork.Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebAuthnController : ControllerBase
    {
        private readonly IFido2 _fido2;
        private readonly UserManager<NexaWorkUser> _userManager;
        private readonly SignInManager<NexaWorkUser> _signInManager;
        private readonly NexaWorkIdentityDbContext _context;

        public WebAuthnController(IFido2 fido2, UserManager<NexaWorkUser> userManager, SignInManager<NexaWorkUser> signInManager, NexaWorkIdentityDbContext context)
        {
            _fido2 = fido2;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpPost("makeCredentialOptions")]
        [Authorize]
        public async Task<JsonResult> MakeCredentialOptions()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return new JsonResult(new { status = "error", errorMessage = "Not logged in" });

            // Get existing keys
            var existingKeys = await _context.FidoStoredCredentials
                .Where(c => c.UserId == user.Id)
                .Select(c => new PublicKeyCredentialDescriptor(c.DescriptorId))
                .ToListAsync();

            var fidoUser = new Fido2User
            {
                DisplayName = user.UserName,
                Name = user.Email,
                Id = System.Text.Encoding.UTF8.GetBytes(user.Id) 
            };

            var authenticatorSelection = new AuthenticatorSelection
            {
                ResidentKey = ResidentKeyRequirement.Required,
                UserVerification = UserVerificationRequirement.Preferred
            };

            var options = _fido2.RequestNewCredential(new RequestNewCredentialParams
            {
                User = fidoUser,
                ExcludeCredentials = existingKeys,
                AuthenticatorSelection = authenticatorSelection,
                AttestationPreference = AttestationConveyancePreference.None
            });

            HttpContext.Session.SetString("fido2.attestationOptions", options.ToJson());

            return new JsonResult(options);
        }

        [HttpPost("makeCredential")]
        [Authorize]
        public async Task<JsonResult> MakeCredential([FromBody] AuthenticatorAttestationRawResponse attestationResponse)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return new JsonResult(new { status = "error", errorMessage = "Not logged in" });

            var jsonOptions = HttpContext.Session.GetString("fido2.attestationOptions");
            if (string.IsNullOrEmpty(jsonOptions))
            {
                return new JsonResult(new { status = "error", errorMessage = "Session options missing" });
            }
            var options = CredentialCreateOptions.FromJson(jsonOptions);

            IsCredentialIdUniqueToUserAsyncDelegate callback = async (args, cancellationToken) =>
            {
                var existing = await _context.FidoStoredCredentials.FirstOrDefaultAsync(c => c.DescriptorId == args.CredentialId);
                return existing == null;
            };

            try
            {
                var success = await _fido2.MakeNewCredentialAsync(new MakeNewCredentialParams
                {
                    AttestationResponse = attestationResponse,
                    OriginalOptions = options,
                    IsCredentialIdUniqueToUserCallback = callback
                });

                // Store in DB
                var cred = new FidoStoredCredential
                {
                    UserId = user.Id,
                    DescriptorId = success.Id,
                    PublicKey = success.PublicKey,
                    UserHandle = success.User.Id,
                    SignatureCounter = success.SignCount,
                    CredType = "public-key",
                    RegDate = DateTime.UtcNow,
                    AaGuid = success.AaGuid,
                    DisplayName = "Passkey " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm")
                };
                
                _context.FidoStoredCredentials.Add(cred);
                await _context.SaveChangesAsync();

                return new JsonResult(success);
            }
            catch (Exception e)
            {
                return new JsonResult(new { status = "error", errorMessage = e.Message });
            }
        }

        [HttpPost("assertionOptions")]
        public async Task<JsonResult> AssertionOptions([FromBody] AssertionOptionsRequest request)
        {
            var existingCredentials = new List<PublicKeyCredentialDescriptor>();
            
            if (!string.IsNullOrEmpty(request.Username))
            {
                var user = await _userManager.FindByEmailAsync(request.Username) ?? await _userManager.FindByNameAsync(request.Username);
                if (user != null)
                {
                    var creds = await _context.FidoStoredCredentials.Where(c => c.UserId == user.Id).ToListAsync();
                    existingCredentials = creds.Select(c => new PublicKeyCredentialDescriptor(c.DescriptorId)).ToList();
                }
            }

            // If existingCredentials is empty, it means Discoverable Credentials will be used.
            var options = _fido2.GetAssertionOptions(new GetAssertionOptionsParams
            {
                AllowedCredentials = existingCredentials,
                UserVerification = UserVerificationRequirement.Preferred
            });

            HttpContext.Session.SetString("fido2.assertionOptions", options.ToJson());
            return new JsonResult(options);
        }

        [HttpPost("makeAssertion")]
        public async Task<JsonResult> MakeAssertion([FromBody] AuthenticatorAssertionRawResponse clientResponse)
        {

            var jsonOptions = HttpContext.Session.GetString("fido2.assertionOptions");
            if (string.IsNullOrEmpty(jsonOptions))
            {
                return new JsonResult(new { status = "error", errorMessage = "Session options missing" });
            }
            var options = Fido2NetLib.AssertionOptions.FromJson(jsonOptions);

            var allCreds = await _context.FidoStoredCredentials.Include(c => c.User).ToListAsync();
            
            // In Fido2NetLib v4, clientResponse.RawId is byte[] while clientResponse.Id might be string or byte[].
            var cred = allCreds.FirstOrDefault(c => c.DescriptorId != null && c.DescriptorId.SequenceEqual(clientResponse.RawId));
            if (cred == null)
            {
                return new JsonResult(new { status = "error", errorMessage = "Unknown credentials" });
            }

            IsUserHandleOwnerOfCredentialIdAsync delegateCheck = async (args, cancellationToken) =>
            {
                return cred.UserHandle.SequenceEqual(args.UserHandle);
            };

            try
            {
                var res = await _fido2.MakeAssertionAsync(new MakeAssertionParams
                {
                    AssertionResponse = clientResponse,
                    OriginalOptions = options,
                    StoredPublicKey = cred.PublicKey,
                    StoredSignatureCounter = cred.SignatureCounter,
                    IsUserHandleOwnerOfCredentialIdCallback = delegateCheck
                });

                // Update counter
                cred.SignatureCounter = res.SignCount;
                await _context.SaveChangesAsync();

                // Because this is passwordless, we sign them in if success!
                // Wait, if it's the 2FA branch, we need to complete the TwoFactorAuthenticator sign in.
                // Or if it's the main login page, we do a full sign in.
                // We'll set a custom session variable to mark them as authenticated by FIDO, then the redirect handles it.
                // Instead, we can return success and have the client POST to a standard ASP.NET MVC endpoint to do the cookie signin.
                // Let's store the User ID in session for the MVC controller to finalize.
                HttpContext.Session.SetString("fido2.authenticatedUserId", cred.UserId);

                return new JsonResult(new { status = "ok", errorMessage = "" });
            }
            catch (Exception e)
            {
                return new JsonResult(new { status = "error", errorMessage = e.Message });
            }
        }
    }

    public class AssertionOptionsRequest
    {
        public string? Username { get; set; }
    }
}
