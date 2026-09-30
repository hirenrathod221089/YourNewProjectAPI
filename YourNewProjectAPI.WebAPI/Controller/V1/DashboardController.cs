using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using YourNewProjectAPI.AppCore.Dto;
using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.WebAPI.Controllers.V1;

[ApiVersion("1.0")]
// INHERIT FROM APIBASECONTROLLER: Replaced the duplicate route and api controller lines!
internal sealed class DashboardController(
    IDashboardService dashboardService,
    IValidator<DashboardRequestDto> validator,
    IPdfReportService pdfReportService,
    IDistributedCache cache) : ApiBaseController
{
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        string cacheKey = "Dashboard_Summary_Key";
        var cachedData = await cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            var cachedRows = JsonSerializer.Deserialize<IEnumerable<string>>(cachedData);
            return Ok(new { Source = "High-Speed Memory Cache (Bypassed DB)", Data = cachedRows });
        }

        var databaseRows = await dashboardService.FetchDashboardSummaryAsync();
        var recordList = databaseRows.Split(", ");

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
        };

        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(recordList), cacheOptions);
        return Ok(new { Source = "SQL Server Database (Dapper Pipeline)", Data = recordList });
    }

    [HttpPost("filtered-summary")]
    [Authorize]
    public async Task<IActionResult> GetFilteredSummary([FromBody] DashboardRequestDto request)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new { Success = false, Errors = validationResult.Errors.Select(e => e.ErrorMessage) });
        }

        var databaseRows = await dashboardService.FetchDashboardSummaryAsync();
        return Ok(new { Message = "v1 Filtered Results", Data = databaseRows, Success = true });
    }

    [HttpGet("download-pdf")]
    public async Task<IActionResult> DownloadSummaryReport()
    {
        var databaseRows = await dashboardService.FetchDashboardSummaryAsync();
        var recordList = databaseRows.Split(", ");

        byte[] pdfBytes = pdfReportService.GenerateDashboardSummaryPdf(recordList);
        string fileName = $"Revenue_Summary_{DateTime.Now:yyyyMMdd}.pdf";

        return File(pdfBytes, "application/pdf", fileName);
    }

    [HttpPost("create")]
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
