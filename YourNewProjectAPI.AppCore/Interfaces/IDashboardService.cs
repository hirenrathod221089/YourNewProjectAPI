namespace YourNewProjectAPI.AppCore.Interfaces
{
    public interface IDashboardService
    {
        public Task<int> CreatePatrakEntryAsync(string patrakName);

        // A clean task blueprint returning a mock string data package
        public Task<string> FetchDashboardSummaryAsync();
    }
}
