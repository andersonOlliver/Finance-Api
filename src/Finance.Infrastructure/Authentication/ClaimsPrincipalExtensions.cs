using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Finance.Infrastructure.Authentication;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// The Keycloak user id (JWT "sub" claim) — matches <c>User.IdentityId</c>, not the
    /// application's own <c>User.Id</c>. Depending on the JWT handler's inbound claim mapping,
    /// the claim surfaces either as "sub" or remapped to <see cref="ClaimTypes.NameIdentifier"/>,
    /// so both are checked.
    /// </summary>
    public static string GetIdentityId(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
               principal?.FindFirstValue(ClaimTypes.NameIdentifier) ??
               throw new ApplicationException("User identity is unavailable");
    }
}
