using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.AppCore.Services;

// Injecting the repository contract into the primary constructor!
public class DashboardService(IUnitOfWork unitOfWork) : IDashboardService
{
    public async Task<string> FetchDashboardSummaryAsync()
    {
        // Execute queries through the safe transaction manager layout
        var rows = await unitOfWork.Dashboards.GetRawSummaryCountsAsync(true);
        return string.Join(", ", rows);
    }
}
