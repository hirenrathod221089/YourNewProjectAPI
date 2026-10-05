using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using YourNewProjectAPI.AppCore.Interfaces;
using YourNewProjectAPI.Infrastructure.Repositories;

namespace YourNewProjectAPI.UnitTest;

public sealed class DashboardRepositoryIntegrationTests
{
    // High-performance sandbox database network path target
    private const string TestConnectionString = "Data Source=local\\MSSQL25; Initial Catalog=Patrak;UID=sa;Pwd=sa123;language=British English;Pooling=True;Connection Timeout=0;TrustServerCertificate=True;";

    [Fact]
    [Trait("Category", "Integration")] // ◄ ADD THIS TAG HERE!
    public async Task AddNewPatrakRecordAsync_WhenExecuted_AutomaticallyPersistsCompleteAuditFootprint()
    {
        // 1. ARRANGE: Revert back to using the concrete SqlConnection type so OpenAsync() works perfectly
        using var connection = new SqlConnection(TestConnectionString);
        if (connection.State == ConnectionState.Closed)
        {
            await connection.OpenAsync();
        }

        using var transaction = await connection.BeginTransactionAsync();

        // 2. ARRANGE: Instantiate our mock integration user context wrapper parameters
        var mockUserContext = new FakeIntegrationUserContext(
            userId: 101,
            loginId: "Officer-Hiren",
            ipAddress: "192.168.1.45"
        );

        // 3. ARRANGE: Instantiate your production repository layer passing dependencies cleanly
        var repository = new DashboardRepository(connection, transaction, mockUserContext);
        string uniquePatrakName = $"Integration_Test_Patrak_{Guid.NewGuid().ToString()[..8]}";

        try
        {
            // 4. ACT: Persist the database record update
            int generatedRowId = await repository.AddNewPatrakRecordAsync(uniquePatrakName, isActive: true);

            // 5. ACT: FIXED BY USING STATIC DAPPER INVOCATION: Resolves extension method clashes cleanly!
            string sqlVerify = "SELECT PatrakName, IsActive, CreatedBy, CreatedByIp FROM PatrakRegisters WHERE Id = @Id";
            var persistedRecord = await SqlMapper.QueryFirstOrDefaultAsync<dynamic>(connection, sqlVerify, new { Id = generatedRowId }, transaction);

            // 6. ASSERT: Verify that your automated audit logic silently captured user metrics!
            Assert.True(generatedRowId > 0);
            Assert.NotNull(persistedRecord);
            Assert.Equal(uniquePatrakName, (string)persistedRecord.PatrakName);
            Assert.Equal("Officer-Hiren", (string)persistedRecord.CreatedBy);
            Assert.Equal("192.168.1.45", (string)persistedRecord.CreatedByIp);
        }
        finally
        {
            // 7. CLEANUP: Always rollback integration tests so they never litter your local database tables!
            await transaction.RollbackAsync();
        }
    }
}

// --- MOCK INTEGRATION USER CONTEXT SHIELD ---
internal sealed class FakeIntegrationUserContext(int userId, string loginId, string ipAddress) : IUserContext
{
    public int UserId => userId;
    public string LoginId => loginId;
    public string UserIpAddress => ipAddress;

    // Standard structural interface fallback properties satisfy compilation definitions
    public string UserName => "Test User";
    public string DCode => "01";
    public string TCode => "02";
    public string OfficeCode => "OFF-001";
    public string DesignationCode => "DES-99";
    public string SsoLiveToken => "MockTokenABC123";

    public void SetSessionValue(string key, string value) { }
}
