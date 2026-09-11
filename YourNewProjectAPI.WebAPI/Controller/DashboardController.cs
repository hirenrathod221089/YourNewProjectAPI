using FluentValidation; // 1. Add this using directive at the top
using Microsoft.AspNetCore.Mvc;
using YourNewProjectAPI.AppCore.Dto;
using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.WebAPI.Controllers;

[ApiController]
[Route("v1/[controller]")]
// 2. Inject IValidator<DashboardRequestDto> directly into your Primary Constructor!
internal sealed class DashboardController(
    IDashboardService dashboardService,
    IValidator<DashboardRequestDto> validator) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IEnumerable<string>> GetSummary()
    {
        var databaseRows = await dashboardService.FetchDashboardSummaryAsync();
        return databaseRows.Split(", ");
    }

    [HttpPost("filtered-summary")]
    public async Task<IActionResult> GetFilteredSummary([FromBody] DashboardRequestDto request)
    {
        // 3. EXPLICITLY RUN THE SHIELD: Validate the incoming data packet
        var validationResult = await validator.ValidateAsync(request);

        // 4. If any rule is broken, stop immediately and return an HTTP 400 Bad Request
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                Success = false,
                Title = "One or more validation errors occurred.",
                Status = 400,
                Errors = validationResult.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage })
            });
        }

        // 5. If data is clean, proceed to your core business logic service
        var databaseRows = await dashboardService.FetchDashboardSummaryAsync();

        return Ok(new
        {
            Message = $"Successfully filtered for Module: '{request.ModuleName}' and Year: {request.Year}",
            Data = databaseRows,
            Success = true
        });
    }
}
