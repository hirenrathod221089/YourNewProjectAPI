using FluentMigrator.Runner;
using FluentValidation; // Add at the top
using System.Reflection;
using YourNewProjectAPI.AppCore.Interfaces;
using YourNewProjectAPI.AppCore.Services;
using YourNewProjectAPI.AppCore.Validators;
using YourNewProjectAPI.Infrastructure.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebDI
{
    public static IServiceCollection ConfigureAppCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IDashboardService, DashboardService>();

        // Register all your fluent validators inside AppCore automatically!
        services.AddValidatorsFromAssemblyContaining<DashboardRequestValidator>();

        return services;
    }

    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        string connString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // 1. Register the FluentMigrator runner engine services
        services.AddLogging(c => c.AddFluentMigratorConsole())
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer()
                    .WithGlobalConnectionString(connString)
                    // Explicitly point the scanner to scan your Infrastructure project for migration scripts
                    .WithMigrationsIn(Assembly.Load("YourNewProjectAPI.Infrastructure")));

        // 2. Link your Unit of Work engine as we did before
        services.AddScoped<IUnitOfWork>(provider => new UnitOfWork(connString));

        return services;
    }
}
