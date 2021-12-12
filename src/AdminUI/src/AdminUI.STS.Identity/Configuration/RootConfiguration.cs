using AdminUI.Shared.Configuration.Identity;
using AdminUI.STS.Identity.Configuration.Interfaces;

namespace AdminUI.STS.Identity.Configuration
{
    public class RootConfiguration : IRootConfiguration
    {
        public AdminConfiguration AdminConfiguration { get; } = new AdminConfiguration();
        public FidoConfiguration FidoConfiguration { get; } = new FidoConfiguration();
        public RegisterConfiguration RegisterConfiguration { get; } = new RegisterConfiguration();

    }
}





