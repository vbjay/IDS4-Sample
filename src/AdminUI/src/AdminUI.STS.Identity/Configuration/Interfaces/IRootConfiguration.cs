using AdminUI.Shared.Configuration.Identity;

namespace AdminUI.STS.Identity.Configuration.Interfaces
{
    public interface IRootConfiguration
    {
        AdminConfiguration AdminConfiguration { get; }
        FidoConfiguration FidoConfiguration { get; }
        RegisterConfiguration RegisterConfiguration { get; }

    }
}





