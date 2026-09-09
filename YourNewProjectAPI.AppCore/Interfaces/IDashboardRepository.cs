namespace YourNewProjectAPI.AppCore.Interfaces;

public interface IDashboardRepository
{
    // Updated contract to accept a parameter filter
    Task<IEnumerable<string>> GetRawSummaryCountsAsync(bool isActive);
}
