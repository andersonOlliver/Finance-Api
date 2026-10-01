using Finance.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace Finance.Infrastructure.Authentication;

internal sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public string IdentityId =>
        httpContextAccessor
            .HttpContext?
            .User
            .GetIdentityId() ??
        throw new ApplicationException("User context is unavailable");

    public Guid UserId =>
        httpContextAccessor.HttpContext?.Items[CurrentUserMiddleware.HttpContextItemKey] as Guid?
        ?? throw new ApplicationException("User context is unavailable");
}
