using FluentMigrator.Runner;
using FluentValidation;
using System.Reflection;
using YourNewProjectAPI.AppCore.Interfaces;
using YourNewProjectAPI.AppCore.Services;
using YourNewProjectAPI.AppCore.Validators;
using YourNewProjectAPI.Infrastructure;
using YourNewProjectAPI.Infrastructure.Reporting;
using YourNewProjectAPI.Infrastructure.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebDID  
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

        // 1. Existing FluentMigrator configurations...
        services.AddLogging(c => c.AddFluentMigratorConsole())
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer()
                    .WithGlobalConnectionString(connString)
                    .WithMigrationsIn(Assembly.Load("YourNewProjectAPI.Infrastructure")));

        services.AddScoped<IUnitOfWork>(provider =>
        {
            var userContext = provider.GetRequiredService<IUserContext>();
            return new UnitOfWork(connString, userContext);
        });

        // 3. ADD THIS LINE HERE TO REGISTER YOUR NEW QUESTPDF ENGINES MATRIX:
        services.AddScoped<IPdfReportService, PdfReportService>();

        return services;
    }
}
