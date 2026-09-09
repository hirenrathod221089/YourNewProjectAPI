namespace YourNewProjectAPI.AppCore.Interfaces
{
    public interface IDashboardService
    {
        // A clean task blueprint returning a mock string data package
        Task<string> FetchDashboardSummaryAsync();
    }
}
