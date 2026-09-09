using YourNewProjectAPI.AppCore.Interfaces;
using YourNewProjectAPI.AppCore.Services;
using YourNewProjectAPI.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration; // Add this using directive at the top

namespace Microsoft.Extensions.DependencyInjection;

public static class WebDI
{
    public static IServiceCollection ConfigureAppCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IDashboardService, DashboardService>();
        return services;
    }

    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Extract the connection string safely from appsettings.json
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // 2. Pass the connection string directly into the repository constructor when creating it!
        services.AddScoped<IDashboardRepository>(provider => new DashboardRepository(connectionString));

        return services;
    }
}
