using Finance.Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Finance.Infrastructure.Authentication;

/// <summary>
/// Resolves the application's own <c>User.Id</c> from the authenticated principal's identity id
/// (the Keycloak "sub" claim) once per request, caching it on <see cref="HttpContext.Items"/> so
/// <see cref="UserContext"/> can expose it synchronously without a per-call DB round trip. Also
/// pushes it onto the Serilog <see cref="LogContext"/> as "CurrentUserId" so every log line for
/// the request — including the request-completed summary — can be filtered by user in production.
/// </summary>
public sealed class CurrentUserMiddleware(RequestDelegate next, ILogger<CurrentUserMiddleware> logger)
{
    internal const string HttpContextItemKey = "CurrentUserId";

    public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var identityId = context.User.GetIdentityId();
            var user = await userRepository.GetByIdentityIdAsync(identityId, context.RequestAborted);

            if (user is not null)
            {
                context.Items[HttpContextItemKey] = user.Id;

                using (LogContext.PushProperty("CurrentUserId", user.Id))
                {
                    await next(context);
                    return;
                }
            }

            logger.LogWarning(
                "Authenticated request with identity {IdentityId} has no matching application user",
                identityId);
        }

        await next(context);
    }
}
