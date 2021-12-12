using System.Collections.Generic;

namespace AdminUI.STS.Identity.ViewModels.Manage
{
    public class MfaRegisterModel
    {
        public string UserName { get; set; }
        public List<FidoKeyInfo> Keys { get; set; }

    }
}

