var builder = WebApplication.CreateBuilder(args);

// 1. Dynamically chain your custom split configuration files at startup
builder.Configuration
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile("applogsettings.json", optional: true, reloadOnChange: true)
       .AddJsonFile("authsettings.json", optional: true, reloadOnChange: true)
       .AddJsonFile("appgeosettings.json", optional: true, reloadOnChange: true)
       .AddEnvironmentVariables(prefix: "YourNewProjectAPI_"); // Matches your company structure

// 2. Register your controllers and swagger tools
builder.Services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
                });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Activate your dependency extensions (Passing configuration down)
builder.Services.ConfigureAppCoreServices()
                .ConfigureInfrastructureServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
