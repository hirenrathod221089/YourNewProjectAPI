using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.WebAPI.Controllers.V2;

[ApiVersion("2.0")]
// INHERIT FROM APIBASECONTROLLER: Replaced the duplicate route and api controller lines!
internal sealed class DashboardController(IDashboardService dashboardService) : ApiBaseController
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
