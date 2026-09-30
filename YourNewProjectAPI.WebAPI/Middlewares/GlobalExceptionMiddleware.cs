using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc; // Add this for ProblemDetails
using System.Net;
using System.Text.Json;

namespace YourNewProjectAPI.WebAPI.Middlewares;

internal sealed class GlobalExceptionMiddleware(RequestDelegate next, Serilog.ILogger logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            // 1. Log the full detailed crash tracking parameters locally via Serilog
            logger.Error(ex, "An unhandled exception occurred during Request: {RequestPath}", context.Request.Path);

            // 2. Clear response buffers and configure the standard Content Type headers
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // 3. Compile the standard corporate RFC 7807 payload envelope
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://yourcompanyportal.com",
                Title = "An unexpected error occurred while processing your request.",
                Detail = "Our engineering team has been automatically notified. Please try again later.",
                Instance = context.Request.Path
            };

            // 4. Serialize and stream the payload packet back to the browser window
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            string jsonResult = JsonSerializer.Serialize(problemDetails, jsonOptions);

            await context.Response.WriteAsync(jsonResult);
        }
    }
}
