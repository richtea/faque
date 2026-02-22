using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Faque.Common.OpenApi;

public sealed class ProblemDetailsExamplesTransformer : IOpenApiSchemaTransformer
{
    /// <inheritdoc />
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (context.JsonTypeInfo.Type != typeof(ProblemDetails))
        {
            return Task.CompletedTask;
        }

        if (schema.Properties == null)
        {
            return Task.CompletedTask;
        }

        schema.Description ??= "An RFC9457-compatible error response.";

        foreach (var (propertyName, propertySchema) in schema.Properties)
        {
            if ((propertySchema.Example != null) || (propertySchema.Examples != null))
            {
                // Don't overwrite examples
                continue;
            }

            if (propertySchema is not OpenApiSchema s)
            {
                continue;
            }

            object? example = propertyName switch
            {
                "type" => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                "title" => "Not Found",
                "status" => 404,
                "detail" => "No route configured for GET /does/not/exist",
                "instance" => "/$$/api/routes/GET/does/not/exist",
                _ => null,
            };

            s.Example = JsonValue.Create(example);
        }

        return Task.CompletedTask;
    }
}
