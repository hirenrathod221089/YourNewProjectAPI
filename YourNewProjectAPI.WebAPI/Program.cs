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

// Enable Serilog request logging middleware to track HTTP processing speeds automatically
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<YourNewProjectAPI.WebAPI.Middlewares.GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
