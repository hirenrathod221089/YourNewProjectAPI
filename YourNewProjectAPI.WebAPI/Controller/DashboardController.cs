using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization; // 1. Add this using directive at the top
using Microsoft.AspNetCore.Mvc;
using YourNewProjectAPI.AppCore.Dto;
using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.WebAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("v{v:apiVersion}/[controller]")]
internal sealed class DashboardController(
    IDashboardService dashboardService,
    IValidator<DashboardRequestDto> validator) : ControllerBase
{
    // --- VERSION 1 ENDPOINTS ---

    [HttpGet("summary")]
    [MapToApiVersion("1.0")]
    public async Task<IEnumerable<string>> GetSummary()
    {
        var databaseRows = await dashboardService.FetchDashboardSummaryAsync();
        return databaseRows.Split(", ");
    }

    [HttpPost("filtered-summary")]
    [MapToApiVersion("1.0")]
    [Authorize] // 2. SHIELD 1: User must simply be authenticated to hit this v1 endpoint
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

    [HttpGet("summary")] // Maps the specific path verb token
    [MapToApiVersion("2.0")] // Binds the action route strictly to your v2 engine profile
    [Authorize(Roles = "Chitnish")] // Retain your security firewall boundary
    public async Task<IActionResult> GetSummaryV2()
    {
        var databaseRows = await dashboardService.FetchDashboardSummaryAsync();

        var v2ResponseEnvelope = new
        {
            Version = "2.0-Alpha",
            Timestamp = DateTime.UtcNow,
            TotalRecordsFound = databaseRows.Split(", ").Length,
            Records = databaseRows.Split(", "),
            AccessedByRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
        };

        return Ok(v2ResponseEnvelope);
    }


}
