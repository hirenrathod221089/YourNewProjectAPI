using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.Infrastructure.Repositories;

internal sealed class DashboardRepository : IDashboardRepository
{
    private readonly string _connectionString;

    // The connection string is now cleanly injected from the outside!
    public DashboardRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<string>> GetRawSummaryCountsAsync(bool isActive)
    {
        using IDbConnection db = new SqlConnection(_connectionString);
        string sqlQuery = "SELECT CategoryNameEng FROM PatrakCategoryTbl WHERE IsActive = @Active";

        return await db.QueryAsync<string>(sqlQuery, new { Active = isActive });
    }
}