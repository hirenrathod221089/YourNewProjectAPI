namespace YourNewProjectAPI.AppCore.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // List all your domain repositories here as view-only targets
    IDashboardRepository Dashboards { get; }

    void BeginTransaction();
    void Commit();
    void Rollback();
}
