using Dapper;
using System.Data;
using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.Infrastructure.Repositories;

// Enforces explicit internal visibility rules matching your team's CA1515 settings
internal sealed class UserRepository(IDbConnection connection, IDbTransaction transaction) : IUserRepository
{
    public async Task<string?> GetPasswordHashByLoginIdAsync(string loginId)
    {
        // High-speed parameterized SQL selector to isolate credentials safely
        string sqlQuery = "SELECT PasswordHash FROM UserProfiles WHERE LoginId = @LoginId AND IsActive = 1";

        return await connection.QueryFirstOrDefaultAsync<string>(sqlQuery, new { LoginId = loginId }, transaction);
    }
}
