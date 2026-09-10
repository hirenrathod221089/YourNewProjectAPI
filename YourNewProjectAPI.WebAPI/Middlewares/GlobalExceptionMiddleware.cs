using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace YourNewProjectAPI.WebAPI.Middlewares;

internal sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context); // Send request to the next step (Controller)
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled execution error occurred in the pipeline.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var errorEnvelope = new
        {
            Success = false,
            Data = (string)null,
            Message = "Internal Server Error. Please contact your administrator.",
            Diagnostics = exception.Message // Mask this line in production env environments
        };

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return context.Response.WriteAsync(JsonSerializer.Serialize(errorEnvelope, jsonOptions));
    }
}
