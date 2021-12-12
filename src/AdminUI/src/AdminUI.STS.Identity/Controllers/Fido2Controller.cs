using AdminUI.Admin.EntityFramework.Shared.Entities.Identity;
using AdminUI.STS.Identity.Configuration.Interfaces;
using AdminUI.STS.Identity.Services;

using Fido2NetLib;
using Fido2NetLib.Objects;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Fido2NetLib.Fido2;

namespace AdminUI.STS.Identity.Controllers
{
    [Route("api/[controller]")]
    public class Fido2Controller<TUser, TKey> : Controller
        where TUser : IdentityUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private readonly Fido2 _lib;
        //private static IMetadataService _mds;
        private readonly Fido2Storage _fido2Storage;
        private readonly UserManager<TUser> _userManager;
        private readonly SignInManager<TUser> _signInManager;
        private readonly IRootConfiguration _config;



        public Fido2Controller(
            Fido2Storage fido2Storage,
            UserManager<TUser> userManager,
            IRootConfiguration config,
            SignInManager<TUser> signInManager
            )
        {
            _userManager = userManager;
            _config = config;
            _fido2Storage = fido2Storage;
            _signInManager = signInManager;

            _lib = new Fido2(_config.FidoConfiguration);
        }

        private string FormatException(Exception e)
        {
            return string.Format("{0}{1}", e.Message, e.InnerException != null ? " (" + e.InnerException.Message + ")" : "");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/mfamakeCredentialOptions")]
        public async Task<JsonResult> MakeCredentialOptions([FromForm] string username, [FromForm] string displayName, [FromForm] string attType, [FromForm] string authType, [FromForm] bool requireResidentKey, [FromForm] string userVerification)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    username = $"{displayName} (Usernameless user created at {DateTime.UtcNow})";
                }


                var identityUser = await _userManager.FindByNameAsync(username);
                var user = new Fido2User
                {
                    DisplayName = identityUser.UserName,
                    Name = identityUser.UserName,
                    Id = Encoding.UTF8.GetBytes(identityUser.UserName) // byte representation of userID is required
                };

                // 2. Get user existing keys by username
                var items = await _fido2Storage.GetCredentialsByUsername(identityUser.UserName);
                var existingKeys = new List<PublicKeyCredentialDescriptor>();
                foreach (var publicKeyCredentialDescriptor in items)
                {
                    existingKeys.Add(publicKeyCredentialDescriptor.Descriptor);
                }

                // 3. Create options
                var authenticatorSelection = new AuthenticatorSelection
                {
                    RequireResidentKey = requireResidentKey,
                    UserVerification = userVerification.ToEnum<UserVerificationRequirement>()

                };

                if (!string.IsNullOrEmpty(authType))
                {
                    authenticatorSelection.AuthenticatorAttachment = authType.ToEnum<AuthenticatorAttachment>();
                }

                var exts = new AuthenticationExtensionsClientInputs()
                {
                    Extensions = true,
                    UserVerificationMethod = true
                };

                var options = _lib.RequestNewCredential(user, existingKeys, authenticatorSelection, attType.ToEnum<AttestationConveyancePreference>(), exts);

                // 4. Temporarily store options, session/in-memory cache/redis/db
                HttpContext.Session.SetString("fido2.attestationOptions", options.ToJson());

                // 5. return options to client
                return Json(options);
            }
            catch (Exception e)
            {
                return Json(new CredentialCreateOptions { Status = "error", ErrorMessage = FormatException(e) });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/mfamakeCredential")]
        public async Task<JsonResult> MakeCredential([FromBody] AuthenticatorAttestationRawResponse attestationResponse)
        {
            try
            {
                // 1. get the options we sent the client
                var jsonOptions = HttpContext.Session.GetString("fido2.attestationOptions");
                var options = CredentialCreateOptions.FromJson(jsonOptions);

                // 2. Create callback so that lib can verify credential id is unique to this user
                IsCredentialIdUniqueToUserAsyncDelegate callback = async (IsCredentialIdUniqueToUserParams args) =>
                {
                    var users = await _fido2Storage.GetUsersByCredentialIdAsync(args.CredentialId);
                    if (users.Count > 0)
                    {
                        return false;
                    }

                    return true;
                };

                // 2. Verify and make the credentials
                var success = await _lib.MakeNewCredentialAsync(attestationResponse, options, callback);
                if (success.Status == "ok") { }
                // 3. Store the credentials in db
                await _fido2Storage.AddCredentialToUser(options.User, new FidoStoredCredential
                {
                    Username = options.User.Name,
                    Descriptor = new PublicKeyCredentialDescriptor(success.Result.CredentialId),
                    PublicKey = success.Result.PublicKey,
                    UserHandle = success.Result.User.Id,
                    SignatureCounter = success.Result.Counter,
                    CredType = success.Result.CredType,
                    RegDate = DateTimeOffset.Now,
                    AaGuid = success.Result.Aaguid
                });

                // 4. return "ok" to the client
                HttpContext.Session.Remove("fido2.attestationOptions");
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {

                    var errorMessage = $"Unable to load user with ID '{_userManager.GetUserId(User)}'.";
                    return base.Json(new CredentialMakeResult("error", errorMessage, new AttestationVerificationSuccess() { ErrorMessage = errorMessage, Status = "error" }));
                }


                await _userManager.SetTwoFactorEnabledAsync(user, true);

                return Json(success);
            }
            catch (Exception e)
            {
                var errorMessage = FormatException(e);
                return base.Json(new CredentialMakeResult("error", errorMessage, new AttestationVerificationSuccess() { ErrorMessage = errorMessage, Status = "error" }));
            }
        }
    }
}
