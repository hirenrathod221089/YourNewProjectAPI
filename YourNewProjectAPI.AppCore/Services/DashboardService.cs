using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.AppCore.Services;

// Injecting the repository contract into the primary constructor!
public class DashboardService(IDashboardRepository dashboardRepository) : IDashboardService
{
    public async Task<string> FetchDashboardSummaryAsync()
    {
        // Passing the parameter down the chain
        var databaseRows = await dashboardRepository.GetRawSummaryCountsAsync(isActive: false);

        var rowList = string.Join(", ", databaseRows);

        if (string.IsNullOrEmpty(rowList))
        {
            return "Connected to infrastructure, but no active Patrak rows were found.";
        }

        return $"Active Patrak Records found via Dapper: {rowList}";
    }
}
