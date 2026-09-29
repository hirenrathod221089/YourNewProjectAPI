using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.WebAPI.Controllers.V2;

[ApiController]
[ApiVersion("2.0")] // 1. Binds this entire file to Version 2 only
[Route("v{v:apiVersion}/[controller]")]
internal sealed class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet("summary")]
    [Authorize(Roles = "Chitnish")]
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
