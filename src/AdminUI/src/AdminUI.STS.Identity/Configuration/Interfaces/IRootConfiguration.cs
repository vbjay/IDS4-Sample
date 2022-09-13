using Skoruba.IdentityServer4.Shared.Configuration.Configuration.Identity;

namespace AdminUI.STS.Identity.Configuration.Interfaces
{
    public interface IRootConfiguration
    {
        AdminConfiguration AdminConfiguration { get; }

        RegisterConfiguration RegisterConfiguration { get; }
    }
}







