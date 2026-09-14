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

await app.RunAsync();
