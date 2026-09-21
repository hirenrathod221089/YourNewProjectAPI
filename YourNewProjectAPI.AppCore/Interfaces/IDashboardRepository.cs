namespace YourNewProjectAPI.AppCore.Interfaces;

public interface IDashboardRepository
{
    public Task<IEnumerable<string>> GetRawSummaryCountsAsync(bool isActive);

    // ADD THIS NEW WRITE CONTRACT DEFINITION HERE:
    public Task<int> AddNewPatrakRecordAsync(string patrakName, bool isActive);
}
