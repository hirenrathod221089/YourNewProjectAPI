using Asp.Versioning;
using Microsoft.AspNetCore.Authentication;
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

// UPDATE YOUR AUTHENTICATION REGISTRATION MATRIX TO THIS:
// REGISTER YOUR HIGH-PERFORMANCE SECURITY SCHEME ENGINE HERE:
builder.Services.AddAuthentication("SimulatedAuth")
                .AddScheme<AuthenticationSchemeOptions, YourNewProjectAPI.WebAPI.Security.SimulatedAuthHandler>("SimulatedAuth", null);

builder.Services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
                });

// ADD THIS HIGH-PERFORMANCE MEMORY CACHING ENGINE REGISTRATION:
builder.Services.AddDistributedMemoryCache(); // Allocates a fast, isolated cache grid in application memory

// 1. Register your versioning tools as we did before
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new Asp.Versioning.UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// 2. Add these two lines to hook up your new configuration helper class!
builder.Services.AddTransient<Microsoft.Extensions.Options.IConfigureOptions<Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions>, YourNewProjectAPI.WebAPI.ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    // Enable the secure 'Bearer Token' security definition layout using native OpenApi types
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Enter your simulated tracking token below. Example: 'Bearer tracking-token-abc'"
    });
});



builder.Services.ConfigureAppCoreServices()
                .ConfigureInfrastructureServices(builder.Configuration);

// 1. READ YOUR TRUSTED FRONTLINE DOMAINS FROM CONFIGURATION
var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? [];

// 2. REGISTER THE CORS SHIELD POLICY INSIDE ENVIRONMENT SERVICE MEMORY
builder.Services.AddCors(options =>
{
    options.AddPolicy("EnterpriseCorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins) // Accept only your trusted sites
              .AllowAnyMethod()            // Allow GET, POST, PUT, DELETE
              .AllowAnyHeader()            // Allow custom auth headers like X-User-Id
              .AllowCredentials();         // Secure cookie tracking bounds pass support
    });
});

var app = builder.Build();

// 1. Core Exception Interceptor
app.UseMiddleware<YourNewProjectAPI.WebAPI.Middlewares.GlobalExceptionMiddleware>();

// 2. Custom User Tracer (FIXED: Clean namespace shortcut)
app.UseMiddleware<YourNewProjectAPI.WebAPI.Middlewares.AuditLoggingMiddleware>();

// 3. Serilog Network Request Logger
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    // UPDATE YOUR SWAGGER UI BLOCK TO THIS PERFECTLY MOUNTED ROUTE GENERATOR:
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();

        foreach (var description in descriptions)
        {
            // USE description.GroupName DIRECTLY: This dynamically creates the exact casing required (/V1/ or /V2/)
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant(); // Displays clean "V1" or "V2" text strings inside the selector

            options.SwaggerEndpoint(url, name);
        }
    });
}

app.UseHttpsRedirection();

// ENSURE THESE TWO LINES ARE SITTING IN THIS EXACT ORDER:
app.UseRouting();
app.UseCors("EnterpriseCorsPolicy"); // ◄ MUST sit between Routing and Authentication!
app.UseAuthentication(); // 1. Reads your custom SimulatedAuthFilter claims pass!
app.UseAuthorization(); // Enables attribute evaluation map parsing

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

