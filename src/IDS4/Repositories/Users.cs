using System.Collections.Generic;
using System.Security.Claims;

using IdentityModel;

using IdentityServer4.Test;

internal class Users
{
    public static List<TestUser> Get() => new List<TestUser> {
            new TestUser {
                SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                Username = "scott",
                Password = "Password123!",
                Claims = new List<Claim> {
                    new Claim(JwtClaimTypes.Email, "scott@scottbrady91.com"),
                    new Claim(JwtClaimTypes.Role, "admin"),
                    new Claim(JwtClaimTypes.Name, "Scott Brady")
                }
            }
        };
}