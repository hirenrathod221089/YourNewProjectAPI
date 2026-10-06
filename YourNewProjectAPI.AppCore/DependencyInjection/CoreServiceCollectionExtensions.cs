using Microsoft.Extensions.DependencyInjection; // ◄ ADD THIS CRITICAL LINE AT THE TOP!
using YourNewProjectAPI.AppCore.Interfaces;
using YourNewProjectAPI.AppCore.Services;

namespace YourNewProjectAPI.AppCore.DependencyInjection;

public static class CoreServiceCollectionExtensions
{
    // Public registration bridge that operates inside the AppCore visibility zone
    public static IServiceCollection AddApplicationCoreServices(this IServiceCollection services)
    {
        // Because this runs inside AppCore, it resolves internal sealed classes perfectly!
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
