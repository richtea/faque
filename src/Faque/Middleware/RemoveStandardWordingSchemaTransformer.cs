using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Faque.Middleware;

/// <summary>
/// A schema transformer that removes any of the standard StyleCop SA1623 prefixes from property descriptions, e.g.
/// <c>Gets or sets</c>.
/// </summary>
public class RemoveStandardWordingSchemaTransformer : IOpenApiSchemaTransformer
{
    private static readonly string[] StandardPropertyPrefixes = ["Gets or sets ", "Gets ", "Sets "];

    /// <inheritdoc />
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (schema.Properties == null)
        {
            return Task.CompletedTask;
        }

        foreach (var (_, propertySchema) in schema.Properties)
        {
            if (propertySchema.Description == null)
            {
                continue;
            }

            var prefix = StandardPropertyPrefixes.FirstOrDefault(p =>
                propertySchema.Description.StartsWith(p, StringComparison.OrdinalIgnoreCase));

            if (prefix == null)
            {
                continue;
            }

            var replacement = propertySchema.Description[prefix.Length..];
            if (replacement.Length > 0)
            {
                replacement = string.Concat(char.ToUpperInvariant(replacement[0]), replacement[1..]);
            }

            propertySchema.Description = replacement;
        }

        return Task.CompletedTask;
    }
}
