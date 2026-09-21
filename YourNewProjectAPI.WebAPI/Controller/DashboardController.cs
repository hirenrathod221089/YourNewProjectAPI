using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourNewProjectAPI.AppCore.Dto;
using YourNewProjectAPI.AppCore.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace YourNewProjectAPI.WebAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("v{v:apiVersion}/[controller]")]
internal sealed class DashboardController(
    IDashboardService dashboardService,
    IValidator<DashboardRequestDto> validator,
    IPdfReportService pdfReportService,
    IDistributedCache cache) : ControllerBase
{
    // --- VERSION 1 ENDPOINTS ---

    [HttpGet("summary")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetSummary()
    {
        string cacheKey = "Dashboard_Summary_Key";

        // 3. Try to fetch data from the high-speed memory cache first
        var cachedData = await cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            var cachedRows = JsonSerializer.Deserialize<IEnumerable<string>>(cachedData);
            return Ok(new { Source = "High-Speed Memory Cache (Bypassed DB)", Data = cachedRows });
        }

        // 4. Cache Miss: Fetch fresh data from Dapper if memory is empty
        var databaseRows = await dashboardService.FetchDashboardSummaryAsync();
        var recordList = databaseRows.Split(", ");

        // 5. Configure cache expiration guidelines (Keep in memory for 60 seconds)
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
        };

        // 6. Save the serialized copy down into the cache grid
        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(recordList), cacheOptions);

        return Ok(new { Source = "SQL Server Database (Dapper Pipeline)", Data = recordList });
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

    [HttpPost("create")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> CreateRecord([FromBody] string patrakName)
    {
        if (string.IsNullOrWhiteSpace(patrakName))
        {
            return BadRequest(new { Success = false, Message = "Name cannot be empty." });
        }

        int generatedId = await dashboardService.CreatePatrakEntryAsync(patrakName);

        return Ok(new
        {
            Success = true,
            Message = $"Successfully inserted record '{patrakName}' with generated ID row number: {generatedId}!"
        });
    }



}
