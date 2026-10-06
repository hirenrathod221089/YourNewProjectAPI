namespace YourNewProjectAPI.AppCore.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // List all your domain repositories here as view-only targets
    public IDashboardRepository Dashboards { get; }

    // ADD THIS LINE HERE TO REGISTER YOUR NEW USER ACCESS REPOSITORY CONTRACT:
    public IUserRepository Users { get; }

    public void BeginTransaction();
    public void Commit();
    public void Rollback();
}
