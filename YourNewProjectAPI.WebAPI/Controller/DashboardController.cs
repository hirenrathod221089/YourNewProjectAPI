using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
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
    IValidator<DashboardRequestDto> validator,
    IPdfReportService pdfReportService) : ControllerBase
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

    // --- BRAND NEW PDF REPORT DOWNLOAD ACTION ---
    [HttpGet("download-pdf")]
    [MapToApiVersion("1.0")] // Let's make it easily accessible to anyone on Version 1.0!
    public async Task<IActionResult> DownloadSummaryReport()
    {
        // 1. Fetch your actual database record string collection values from your service layer
        var databaseRows = await dashboardService.FetchDashboardSummaryAsync();
        var recordList = databaseRows.Split(", ");

        // 2. Execute your QuestPDF layout engine to compile the binary report packet stream
        byte[] pdfBytes = pdfReportService.GenerateDashboardSummaryPdf(recordList);

        // 3. Stream the file binary back to the web browser as a real downloadable attachment file document
        string fileName = $"Revenue_Summary_{DateTime.Now:yyyyMMdd}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }


}
