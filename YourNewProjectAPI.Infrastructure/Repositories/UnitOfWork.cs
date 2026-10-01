using System.Data;
using Microsoft.Data.SqlClient;
using YourNewProjectAPI.AppCore.Interfaces; // 1. Ensure this namespace is present at the top
using YourNewProjectAPI.Infrastructure.Repositories;

namespace YourNewProjectAPI.Infrastructure;

// 2. UPDATE YOUR PRIMARY CONSTRUCTOR: Inject IUserContext here too!
public sealed class UnitOfWork(string connectionString, IUserContext userContext) : IUnitOfWork
{
    private readonly IDbConnection _connection = new SqlConnection(connectionString);
    private IDbTransaction? _transaction;

    // 3. UPDATE THE REPOSITORY PROPERTY FACTORY LINE:
    // Pass userContext as the third parameter here!
    // Add the '!' symbol right after _transaction to clear the warning:
    public IDashboardRepository Dashboards => new DashboardRepository(_connection, _transaction!, userContext);

    public IDbConnection Connection => _connection;
    public IDbTransaction? Transaction => _transaction;

    public void BeginTransaction()
    {
        if (_connection.State == ConnectionState.Closed)
        {
            _connection.Open();
        }
        _transaction = _connection.BeginTransaction();
    }

    public void Commit()
    {
        _transaction?.Commit();
        DisposeTransaction();
    }

    public void Rollback()
    {
        _transaction?.Rollback();
        DisposeTransaction();
    }

    private void DisposeTransaction()
    {
        _transaction?.Dispose();
        _transaction = null;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection.Dispose();
    }
}
