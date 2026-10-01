using Dapper;
using System.Data;
using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.Infrastructure.Repositories;

// ENFORCES PRIMARY CONSTRUCTORS: Inject IUserContext straight into the repository!
internal sealed class DashboardRepository(
    IDbConnection connection,
    IDbTransaction transaction,
    IUserContext userContext) : IDashboardRepository // ◄ Added userContext tracking handle here
{
    public async Task<IEnumerable<string>> GetRawSummaryCountsAsync(bool isActive)
    {
        string sqlQuery = "SELECT PatrakName FROM PatrakRegisters WHERE IsActive = @ActiveFilter";
        return await connection.QueryAsync<string>(sqlQuery, new { ActiveFilter = isActive }, transaction);
    }

    public async Task<int> AddNewPatrakRecordAsync(string patrakName, bool isActive)
    {
        // AUTOMATED AUDITING: The insert query maps audit columns implicitly!
        string sqlInsert = @"INSERT INTO PatrakRegisters (PatrakName, IsActive, CreatedBy, CreatedDate, CreatedByIp) 
                             VALUES (@Name, @ActiveStatus, @User, @Date, @Ip);
                             SELECT CAST(SCOPE_IDENTITY() as int);";

        // Read active officer parameters silently behind the scenes without manual controller parameters parameters overhead!
        var queryParameters = new
        {
            Name = patrakName,
            ActiveStatus = isActive,
            User = userContext.LoginId ?? "System-Fallback",
            Date = DateTime.UtcNow,
            Ip = userContext.UserIpAddress ?? "127.0.0.1"
        };

        return await connection.ExecuteScalarAsync<int>(sqlInsert, queryParameters, transaction);
    }
}
