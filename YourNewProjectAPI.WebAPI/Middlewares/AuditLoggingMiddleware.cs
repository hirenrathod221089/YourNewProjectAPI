using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace YourNewProjectAPI.WebAPI.Middlewares;

internal sealed class AuditLoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Simulate reading user data from request headers or auth context
        // In your production portal, this reads your session tokens (e.g., Chitnish/Officer IDs)
        string trackingUserId = context.Request.Headers["X-User-Id"].FirstOrDefault() ?? "Anonymous-User";
        string regionalCorrelationId = Guid.NewGuid().ToString()[..8]; // Clean short unique track ID

        // 2. Use Serilog's LogContext to PUSH custom properties into the logging scope
        using (LogContext.PushProperty("TrackingUser", trackingUserId))
        using (LogContext.PushProperty("CorrelationId", regionalCorrelationId))
        {
            // 3. Pass the execution along the pipeline. 
            // Any log entry written down this chain will automatically inherit these properties!
            await next(context);
        }
    }
}
