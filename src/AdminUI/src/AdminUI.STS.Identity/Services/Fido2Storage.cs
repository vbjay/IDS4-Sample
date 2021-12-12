using AdminUI.Admin.EntityFramework.Shared.DbContexts;
using AdminUI.Admin.EntityFramework.Shared.Entities.Identity;

using Fido2NetLib;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminUI.STS.Identity.Services
{
    public class Fido2Storage
    {
        private readonly AdminIdentityDbContext _IDDbContext;


        public Fido2Storage(AdminIdentityDbContext applicationDbContext)
        {
            _IDDbContext = applicationDbContext;
        }

        public async Task<List<FidoStoredCredential>> GetCredentialsByUsername(string username)
        {
            return await _IDDbContext.FidoCredentials.Where(c => c.Username == username).ToListAsync();
        }

        public async Task RemoveCredentialsByUsername(string username)
        {
            var items = await _IDDbContext.FidoCredentials.Where(c => c.Username == username).ToListAsync();
            if (items != null)
            {
                foreach (var fido2Key in items)
                {
                    _IDDbContext.FidoCredentials.Remove(fido2Key);


                };

                await _IDDbContext.SaveChangesAsync();
            }
        }


        public async Task RemoveCredential(byte[] id, string userName)
        {
            var credentialIdString = Base64Url.Encode(id);

            var cred = await _IDDbContext.FidoCredentials.FirstOrDefaultAsync(c => !string.IsNullOrWhiteSpace(c.DescriptorJson)
                                                                                   && c.Username == userName
                                                                                   && c.DescriptorJson.Contains(credentialIdString));
            if (cred != null)
            {
                _IDDbContext.FidoCredentials.Remove(cred);
                await _IDDbContext.SaveChangesAsync();
            }
        }
        public async Task<FidoStoredCredential> GetCredentialById(byte[] id)
        {
            var credentialIdString = Base64Url.Encode(id);
            //byte[] credentialIdStringByte = Base64Url.Decode(credentialIdString);

            var cred = await _IDDbContext.FidoCredentials
                .Where(c => c.DescriptorJson.Contains(credentialIdString)).FirstOrDefaultAsync();

            return cred;
        }

        public Task<List<FidoStoredCredential>> GetCredentialsByUserHandleAsync(byte[] userHandle)
        {
            return Task.FromResult(_IDDbContext.FidoCredentials.Where(c => c.UserHandle.SequenceEqual(userHandle)).ToList());
        }

        public async Task UpdateCounter(byte[] credentialId, uint counter)
        {
            var credentialIdString = Base64Url.Encode(credentialId);
            //byte[] credentialIdStringByte = Base64Url.Decode(credentialIdString);

            var cred = await _IDDbContext.FidoCredentials
                .Where(c => c.DescriptorJson.Contains(credentialIdString)).FirstOrDefaultAsync();

            cred.SignatureCounter = counter;
            await _IDDbContext.SaveChangesAsync();
        }

        public async Task<FidoStoredCredential> UpdateDescription(byte[] credentialId, string description)
        {
            var credentialIdString = Base64Url.Encode(credentialId);
            //byte[] credentialIdStringByte = Base64Url.Decode(credentialIdString);

            var cred = await _IDDbContext.FidoCredentials
                .Where(c => c.DescriptorJson.Contains(credentialIdString)).FirstOrDefaultAsync();

            cred.Description = description;
            await _IDDbContext.SaveChangesAsync();
            return cred;
        }

        public async Task AddCredentialToUser(Fido2User user, FidoStoredCredential credential)
        {
            credential.UserId = user.Id;
            _IDDbContext.FidoCredentials.Add(credential);
            await _IDDbContext.SaveChangesAsync();
        }

        public async Task<List<Fido2User>> GetUsersByCredentialIdAsync(byte[] credentialId)
        {
            var credentialIdString = Base64Url.Encode(credentialId);
            //byte[] credentialIdStringByte = Base64Url.Decode(credentialIdString);

            var cred = await _IDDbContext.FidoCredentials
                .Where(c => c.DescriptorJson.Contains(credentialIdString)).FirstOrDefaultAsync();

            if (cred == null)
            {
                return new List<Fido2User>();
            }

            return await _IDDbContext.Users
                    .Where(u => Encoding.UTF8.GetBytes(u.UserName)
                    .SequenceEqual(cred.UserId))
                    .Select(u => new Fido2User
                    {
                        DisplayName = u.UserName,
                        Name = u.UserName,
                        Id = Encoding.UTF8.GetBytes(u.UserName) // byte representation of userID is required
                    }).ToListAsync();
        }
    }
}
