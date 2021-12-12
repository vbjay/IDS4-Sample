using AdminUI.Admin.EntityFramework.Shared.Entities.Identity;

using Microsoft.AspNetCore.Identity;

using System.Threading.Tasks;

namespace AdminUI.STS.Identity.Helpers
{


    public class Fido2UserTwoFactorTokenProvider : IUserTwoFactorTokenProvider<UserIdentity>
    {
        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<UserIdentity> manager, UserIdentity user)
        {
            return Task.FromResult(true);
        }

        public Task<string> GenerateAsync(string purpose, UserManager<UserIdentity> manager, UserIdentity user)
        {
            return Task.FromResult("fido2");
        }

        public Task<bool> ValidateAsync(string purpose, string token, UserManager<UserIdentity> manager, UserIdentity user)
        {
            return Task.FromResult(true);
        }
    }
}
