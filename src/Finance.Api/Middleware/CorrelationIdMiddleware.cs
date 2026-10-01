using Serilog.Context;

namespace Finance.Api.Middleware;

/// <summary>
/// Pushes the request's <see cref="HttpContext.TraceIdentifier"/> onto the Serilog
/// <see cref="LogContext"/> as "CorrelationId" and echoes it back on the response, so a user
/// reporting an error can be matched to the exact logs in Seq.
/// </summary>
public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.TraceIdentifier;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}
