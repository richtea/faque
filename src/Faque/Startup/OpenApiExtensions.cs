using System.Text.Json.Serialization.Metadata;
using Faque.Common.OpenApi;
using Microsoft.AspNetCore.OpenApi;

namespace Faque.Startup;

public static class OpenApiExtensions
{
    private static readonly string[] CommonModelSuffixes = ["Model", "Dto"];

    public static IServiceCollection ConfigureApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info.Title = "Faque API";
                document.Info.Version = "v1";
                document.Info.Description = "API for managing Fake API routes and viewing captured requests.";
                return Task.CompletedTask;
            });

            options
                .AddSchemaTransformer<RemoveStandardWordingSchemaTransformer>()
                .AddSchemaTransformer<ProblemDetailsExamplesTransformer>()
                .AddSchemaTransformer<OpenApiNumberDeastonisher>();

            options.CreateSchemaReferenceId = CreateSchemaReferenceId;
        });

        return services;
    }

    /// <summary>
    /// Modifies the schema reference ID by removing common extensions like 'Model' or 'Dto'.
    /// </summary>
    /// <param name="typeInfo">Contains information about the schema type.</param>
    /// <returns>The modified schema reference ID without common extensions.</returns>
    private static string? CreateSchemaReferenceId(JsonTypeInfo typeInfo)
    {
        var defaultName = OpenApiOptions.CreateDefaultSchemaReferenceId(typeInfo);

        if (defaultName == null)
        {
            return null;
        }

        var suffix = CommonModelSuffixes.FirstOrDefault(p =>
            defaultName.EndsWith(p, StringComparison.OrdinalIgnoreCase));

        var name = suffix == null ? defaultName : defaultName[..^suffix.Length];
        return name;
    }
}
