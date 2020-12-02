using System.Collections.Generic;
using IdentityServer4.Models;

internal class Resources
{
    public static IEnumerable<IdentityResource> GetIdentityResources() => new[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource
            {
                Name = "role",
                UserClaims = new List<string> {"role"}
            }
        };

    public static IEnumerable<ApiResource> GetApiResources() => new[]
        {
            new ApiResource
            {
                Name = "api1",
                DisplayName = "API #1",
                Description = "Allow the application to access API #1 on your behalf",
                Scopes = new List<string> {"api1.read", "api1.write"},
                ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
                UserClaims = new List<string> {"role"}
            }
        };

    public static IEnumerable<ApiScope> GetApiScopes() => new[]
        {
            new ApiScope("api1.read", "Read Access to API #1"),
            new ApiScope("api1.write", "Write Access to API #1")
        };
}