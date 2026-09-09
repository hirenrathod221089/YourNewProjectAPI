using YourNewProjectAPI.AppCore.Interfaces;
using YourNewProjectAPI.AppCore.Services;

namespace YourNewProjectAPI.UnitTest;

public class DashboardServiceTests
{
    [Fact]
    public async Task FetchDashboardSummaryAsync_WhenRecordsExist_ReturnsFormattedString()
    {
        // 1. ARRANGE: Set up a fake mock repository behavior
        var mockRecords = new List<string> { "Patrak-1", "Patrak-2" };
        var fakeRepository = new FakeDashboardRepository(mockRecords);

        // Inject the fake repository into the service's primary constructor
        var service = new DashboardService(fakeRepository);

        // 2. ACT: Execute the business logic method
        var result = await service.FetchDashboardSummaryAsync();

        // 3. ASSERT: Verify the text matches our expectations exactly
        Assert.Contains("Active Patrak Records found via Dapper: Patrak-1, Patrak-2", result);
    }

    [Fact]
    public async Task FetchDashboardSummaryAsync_WhenNoRecordsExist_ReturnsNotFoundMessage()
    {
        // ARRANGE: Empty mock list
        var fakeRepository = new FakeDashboardRepository(new List<string>());
        var service = new DashboardService(fakeRepository);

        // ACT: Execute
        var result = await service.FetchDashboardSummaryAsync();

        // ASSERT: Verify the safe fallback string triggers
        Assert.Equal("Connected to infrastructure, but no active Patrak rows were found.", result);
    }
}

// A simple local Fake class acting as a mock database shield for our test suite
internal class FakeDashboardRepository(IEnumerable<string> mockData) : IDashboardRepository
{
    public async Task<IEnumerable<string>> GetRawSummaryCountsAsync(bool isActive)
    {
        return await Task.FromResult(mockData);
    }
}
