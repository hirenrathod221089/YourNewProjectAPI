using System.Data;
using Microsoft.Data.SqlClient;
using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.Infrastructure.Repositories;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private IDbTransaction _transaction;
    private IDashboardRepository _dashboards;

    public UnitOfWork(string connectionString)
    {
        _connection = new SqlConnection(connectionString);
        _connection.Open();
    }

    // Lazy load the repository, passing the active shared connection link into it
    public IDashboardRepository Dashboards => _dashboards ??= new DashboardRepository(_connection, _transaction);

    public void BeginTransaction() => _transaction = _connection.BeginTransaction();

    public void Commit()
    {
        _transaction?.Commit();
        _transaction?.Dispose();
        _transaction = null;
    }

    public void Rollback()
    {
        _transaction?.Rollback();
        _transaction?.Dispose();
        _transaction = null;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection?.Dispose();
    }
}
