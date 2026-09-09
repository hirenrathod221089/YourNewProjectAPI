using Microsoft.AspNetCore.Mvc;
using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.WebAPI.Controllers;

[ApiController]
[Route("v1/[controller]")]
// Using 'internal sealed' and Primary Constructor injection matching your company standard
internal sealed class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        // Calls out to your AppCore service engine room
        var dataResult = await dashboardService.FetchDashboardSummaryAsync();

        return Ok(new { Data = dataResult, Success = true });
    }
}