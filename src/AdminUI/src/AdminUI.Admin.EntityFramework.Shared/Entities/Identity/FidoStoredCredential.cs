using Fido2NetLib.Objects;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminUI.Admin.EntityFramework.Shared.Entities.Identity
{
    public class FidoStoredCredential
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Username { get; set; }
        public byte[] UserId { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] UserHandle { get; set; }
        public uint SignatureCounter { get; set; }
        public string CredType { get; set; }
        public DateTimeOffset RegDate { get; set; }
        public Guid AaGuid { get; set; }

        [NotMapped]
        public PublicKeyCredentialDescriptor Descriptor {
            get {
                try
                {
                    return string.IsNullOrWhiteSpace(DescriptorJson) ? null : System.Text.Json.JsonSerializer.Deserialize<PublicKeyCredentialDescriptor>(DescriptorJson);
                }
                catch { return null; }
            }
            set { DescriptorJson = System.Text.Json.JsonSerializer.Serialize(value); }
        }
        public string DescriptorJson { get; set; }
        public string Description { get; set; }

    }
}
