using Fido2NetLib;
using Fido2NetLib.Objects;

using Microsoft.AspNetCore.WebUtilities;

using System;

namespace AdminUI.STS.Identity.ViewModels.Manage
{
    public class FidoKeyInfo
    {
        public string Description { get; set; }
        public DateTimeOffset Registered { get; set; }
        public PublicKeyCredentialDescriptor Descriptor => string.IsNullOrWhiteSpace(DescriptorJson) ? null : System.Text.Json.JsonSerializer.Deserialize<PublicKeyCredentialDescriptor>(DescriptorJson);
        public string DescriptorJson { get; set; }

        public string ID => WebEncoders.Base64UrlEncode(Descriptor.Id);
        public string FidoID => Base64Url.Encode(Descriptor.Id);// string.Join("", Descriptor.Id.Select(b => b.ToString("X2")));
        public byte[] PublicKey { get; set; }

        string credId;
        public string CredId {
            get => credId;
            set {
                credId = value;
                DescriptorJson = System.Text.Json.JsonSerializer.Serialize(new PublicKeyCredentialDescriptor(Base64Url.Decode(value)));
            }
        }

    }
}

