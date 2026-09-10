using Dapper;
using System.Data;
using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.Infrastructure.Repositories;

internal sealed class DashboardRepository(IDbConnection connection, IDbTransaction transaction) : IDashboardRepository
{
    public async Task<IEnumerable<string>> GetRawSummaryCountsAsync(bool isActive)
    {
        string sqlQuery = "SELECT CategoryNameEng FROM PatrakCategoryTbl WHERE IsActive = @ActiveFilter";

        // Pass the shared transaction instance safely to Dapper
        return await connection.QueryAsync<string>(sqlQuery, new { ActiveFilter = isActive }, transaction);
    }
}
