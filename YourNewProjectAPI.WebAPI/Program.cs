using Serilog; // Add this using directive at the top

var builder = WebApplication.CreateBuilder(args);

// 1. Setup the split configuration files as we did before
builder.Configuration
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile("applogsettings.json", optional: true, reloadOnChange: true)
       .AddJsonFile("authsettings.json", optional: true, reloadOnChange: true)
       .AddJsonFile("appgeosettings.json", optional: true, reloadOnChange: true)
       .AddEnvironmentVariables(prefix: "YourNewProjectAPI_");

// 2. Clear default loggers and inject Serilog using your applogsettings configuration
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Update your AddControllers block to include fluent validation tracking hooks
builder.Services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
                });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureAppCoreServices()
                .ConfigureInfrastructureServices(builder.Configuration);

var app = builder.Build();

// 1. Core Exception Interceptor
app.UseMiddleware<YourNewProjectAPI.WebAPI.Middlewares.GlobalExceptionMiddleware>();

// 2. Custom User Tracer (Must execute before request logger!)
app.UseMiddleware<YourNewProjectAPI.WebAPI.Middlewares.AuditLoggingMiddleware>();

// 3. Serilog Network Request Logger
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Scroll to the bottom of your Program.cs file:

// 1. Read the feature flag value directly from your configuration provider layers
bool runMigrations = builder.Configuration.GetValue<bool>("DatabaseSettings:RunMigrationsAtStartup");

if (runMigrations)
{
    // Locate the migration runner inside your container memory grid
    using (var scope = app.Services.CreateScope())
    {
        var runner = scope.ServiceProvider.GetRequiredService<FluentMigrator.Runner.IMigrationRunner>();

        // Executes all pending schema changes automatically ONLY if true!
        runner.MigrateUp();
    }
}

await app.RunAsync();

