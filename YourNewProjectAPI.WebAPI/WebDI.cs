using FluentMigrator.Runner;
using FluentValidation;
using System.Reflection;
using YourNewProjectAPI.AppCore.DependencyInjection;
using YourNewProjectAPI.AppCore.Interfaces;
using YourNewProjectAPI.AppCore.Services;
using YourNewProjectAPI.AppCore.Validators;
using YourNewProjectAPI.Infrastructure;
using YourNewProjectAPI.Infrastructure.Reporting;
using YourNewProjectAPI.WebAPI;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebDID
{
    public static IServiceCollection ConfigureAppCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IDashboardService, DashboardService>();

        // Register all your fluent validators inside AppCore automatically!
        services.AddValidatorsFromAssemblyContaining<DashboardRequestValidator>();

        services.AddApplicationCoreServices();


        return services;
    }

    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        string connString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        // 1. ADD THIS MAPPING HERE TO FIX THE DI MISSING SERVICE CRASH:
        services.AddScoped<IUserContext, MockUserContext>();

        // 2. Existing FluentMigrator configurations...
        services.AddLogging(c => c.AddFluentMigratorConsole())
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer()
                    .WithGlobalConnectionString(connString)
                    .WithMigrationsIn(Assembly.Load("YourNewProjectAPI.Infrastructure")));

        // 3. This block will now resolve perfectly without throwing exceptions!
        services.AddScoped<IUnitOfWork>(provider =>
        {
            var userContext = provider.GetRequiredService<IUserContext>();
            return new UnitOfWork(connString, userContext);
        });

        services.AddScoped<IPdfReportService, PdfReportService>();

        return services;
    }
}
