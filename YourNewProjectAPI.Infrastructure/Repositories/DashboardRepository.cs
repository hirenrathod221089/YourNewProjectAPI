using Dapper;
using System.Data;
using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.Infrastructure.Repositories;

internal sealed class DashboardRepository(IDbConnection connection, IDbTransaction transaction) : IDashboardRepository
{
    public async Task<IEnumerable<string>> GetRawSummaryCountsAsync(bool isActive)
    {
        string sqlQuery = "SELECT CategoryNameEng FROM PatrakCategoryTbl WHERE IsActive = @ActiveFilter";
        return await connection.QueryAsync<string>(sqlQuery, new { ActiveFilter = isActive }, transaction);
    }

    // IMPLEMENT THE HIGH-SPEED WRITE LOGIC HERE:
    public async Task<int> AddNewPatrakRecordAsync(string patrakName, bool isActive)
    {
        // Enforce parameterized bindings to prevent SQL Injection attempts completely!
        string sqlInsert = $@"INSERT INTO PatrakRegisters (PatrakName, IsActive) 
                             VALUES (@Name, @ActiveStatus);
                             SELECT CAST(SCOPE_IDENTITY() as int);"; // Returns the newly created Id row number

        var queryParameters = new { Name = patrakName, ActiveStatus = isActive };

        return await connection.ExecuteScalarAsync<int>(sqlInsert, queryParameters, transaction);
    }
}
