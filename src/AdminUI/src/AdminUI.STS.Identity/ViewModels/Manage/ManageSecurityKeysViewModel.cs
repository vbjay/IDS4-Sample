using Fido2NetLib.Objects;

using System.Collections.Generic;

namespace AdminUI.STS.Identity.ViewModels.Manage
{
    public class ManageSecurityKeysViewModel
    {
        public List<PublicKeyCredentialDescriptor> Credentials { get; set; }
    }
}
