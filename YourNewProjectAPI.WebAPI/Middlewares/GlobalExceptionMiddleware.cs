using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using FluentValidation; // 1. Add this using statement at the top
using YourNewProjectAPI.AppCore.Dto;

namespace YourNewProjectAPI.WebAPI.Middlewares;

internal sealed class GlobalExceptionMiddleware(RequestDelegate next, Serilog.ILogger logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException valEx) // 2. INTERCEPT FLUENTVALIDATION DROPS SEPARATELY:
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            // Extract the exact validation error errors list parameters dynamically
            var errorDetails = valEx.Errors.Select(e => new ValidationErrorDetail(e.PropertyName, e.ErrorMessage));

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "https://yourcompanyportal.com",
                Title = "One or more validation errors occurred.",
                Detail = "Please refer to the errors property for additional details.",
                Instance = context.Request.Path
            };

            // Inject the custom errors dictionary safely into the standard Problem Details container
            problemDetails.Extensions.Add("errors", errorDetails);

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, jsonOptions));
        }
        catch (Exception ex) // 3. Catch all other regular 500 runtime server errors here
        {
            logger.Error(ex, "An unhandled exception occurred during Request: {RequestPath}", context.Request.Path);

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://yourcompanyportal.com",
                Title = "An unexpected error occurred while processing your request.",
                Detail = "Our engineering team has been automatically notified. Please try again later.",
                Instance = context.Request.Path
            };

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, jsonOptions));
        }
    }
}
