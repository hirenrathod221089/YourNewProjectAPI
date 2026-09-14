using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using YourNewProjectAPI.AppCore.Dto;
using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.WebAPI.Controllers;

[ApiController]
[ApiVersion("1.0")] // Supported Version 1.0
[ApiVersion("2.0")] // Explicitly add support for Version 2.0 on this controller class!
[Route("v{v:apiVersion}/[controller]")]
internal sealed class DashboardController(
    IDashboardService dashboardService,
    IValidator<DashboardRequestDto> validator) : ControllerBase
{
    // --- VERSION 1 ENDPOINTS ---

    [HttpGet("summary")]
    [MapToApiVersion("1.0")] // Maps this specific method strictly to v1
    public async Task<IEnumerable<string>> GetSummary()
    {
        var databaseRows = await dashboardService.FetchDashboardSummaryAsync();
        return databaseRows.Split(", ");
    }

    [HttpPost("filtered-summary")]
    [MapToApiVersion("1.0")] // Maps this specific method strictly to v1
    public async Task<IActionResult> GetFilteredSummary([FromBody] DashboardRequestDto request)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { Success = false, Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        var databaseRows = await dashboardService.FetchDashboardSummaryAsync();
        return Ok(new { Message = $"v1 Filtered Results", Data = databaseRows, Success = true });
    }


    // --- BRAND NEW VERSION 2 ENDPOINT ---

    [HttpGet("summary")]
    [MapToApiVersion("2.0")] // Maps this advanced endpoint strictly to v2!
    public async Task<IActionResult> GetSummaryV2()
    {
        var databaseRows = await dashboardService.FetchDashboardSummaryAsync();

        // Imagine v2 changes the return structure completely to include server metadata diagnostics
        var v2ResponseEnvelope = new
        {
            Version = "2.0-Alpha",
            Timestamp = DateTime.UtcNow,
            TotalRecordsFound = databaseRows.Split(", ").Length,
            Records = databaseRows.Split(", ")
        };

        return Ok(v2ResponseEnvelope);
    }
}
