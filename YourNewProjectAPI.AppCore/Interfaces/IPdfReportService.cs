namespace YourNewProjectAPI.AppCore.Interfaces;

public interface IPdfReportService
{
    byte[] GenerateDashboardSummaryPdf(IEnumerable<string> patrakRecords);
}
