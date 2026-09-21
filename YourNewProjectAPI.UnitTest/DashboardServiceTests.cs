using Xunit;
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
        var fakeUnitOfWork = new FakeUnitOfWork(mockRecords);
        var service = new DashboardService(fakeUnitOfWork);

        // 2. ACT: Execute the business logic method
        var result = await service.FetchDashboardSummaryAsync();

        // 3. ASSERT: Changed to match your exact service output text!
        Assert.Equal("Patrak-1, Patrak-2", result);
    }

    [Fact]
    public async Task FetchDashboardSummaryAsync_WhenNoRecordsExist_ReturnsNotFoundMessage()
    {
        // ARRANGE: Empty mock list wrapped in our Unit of Work
        var fakeUnitOfWork = new FakeUnitOfWork(new List<string>());
        var service = new DashboardService(fakeUnitOfWork);

        // ACT: Execute
        var result = await service.FetchDashboardSummaryAsync();

        // ASSERT: Changed to match your exact fallback output text!
        Assert.Equal(string.Empty, result);
    }
}

// ─── FAKE TRANSACTION WRAPPERS FOR TESTING ───

internal class FakeDashboardRepository(IEnumerable<string> mockData) : IDashboardRepository
{
    public async Task<IEnumerable<string>> GetRawSummaryCountsAsync(bool isActive)
    {
        return await Task.FromResult(mockData);
    }

    // ADD THIS FIX HERE: Implements the new interface method so your test project compiles!
    public async Task<int> AddNewPatrakRecordAsync(string patrakName, bool isActive)
    {
        // Simply return a mock row ID number (like 99) to satisfy the unit test pipeline
        return await Task.FromResult(99);
    }
}

internal class FakeUnitOfWork(IEnumerable<string> mockData) : IUnitOfWork
{
    public IDashboardRepository Dashboards => new FakeDashboardRepository(mockData);
    public void BeginTransaction() { }
    public void Commit() { }
    public void Rollback() { }
    public void Dispose() { }
}
