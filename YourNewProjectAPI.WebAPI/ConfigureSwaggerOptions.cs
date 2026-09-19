using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace YourNewProjectAPI.WebAPI;

internal sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        // Automatically generate a beautiful metadata description block for every version found!
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForVersion(description));
        }
    }

    private static OpenApiInfo CreateInfoForVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo
        {
            Title = $"YourNewProjectAPI WebAPI - {description.GroupName.ToUpperInvariant()}",
            Version = description.ApiVersion.ToString(),
            Description = "Enterprise Clean Architecture Multi-Version Microservice Engine."
        };

        if (description.IsDeprecated)
        {
            info.Description += " (This API version has been deprecated).";
        }

        return info;
    }
}
