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
        string connString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException();

        // Register Unit of Work as Scoped (one connection per web request)
        services.AddScoped<IUnitOfWork>(provider => new UnitOfWork(connString));
        return services;
    }
}
