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

    // ADD THIS NEW WRITE WRAPPER METHOD INSIDE YOUR SERVICE:
    public async Task<int> CreatePatrakEntryAsync(string name)
    {
        // 1. Open a secure transaction boundary block across repositories
        unitOfWork.BeginTransaction();

        try
        {
            // 2. Persist the database record update
            int newRowId = await unitOfWork.Dashboards.AddNewPatrakRecordAsync(name, true);

            // 3. Commit all modifications safely to SQL Server at once
            unitOfWork.Commit();
            return newRowId;
        }
        catch
        {
            // 4. Rollback and drop changes immediately if a hardware crash occurs!
            unitOfWork.Rollback();
            throw;
        }
    }

}
