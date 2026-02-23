using System.Collections;
using System.Diagnostics;
using Faque.Common.AspNet;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Faque.Common.OpenApi;

/// <summary>
/// Modifies the generated OpenAPI schema for dictionary properties that are decorated with the
/// <see cref="KeyValidationRegexAttribute"/> to include a pattern property for validating dictionary keys.
/// </summary>
public class DictionaryKeyValidationTransformer : IOpenApiSchemaTransformer
{
    /// <inheritdoc />
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (!ShouldTransform(schema, context) ||
            schema.AdditionalProperties is not OpenApiSchema additionalProperties)
        {
            return Task.CompletedTask;
        }

        Debug.Assert(context.JsonPropertyInfo?.AttributeProvider != null);

        var attrs = context.JsonPropertyInfo.AttributeProvider.GetCustomAttributes(
            typeof(KeyValidationRegexAttribute),
            false);
        if (attrs.Length == 0)
        {
            return Task.CompletedTask;
        }

        var pattern = ((KeyValidationRegexAttribute)attrs[0]).Pattern;
        additionalProperties.Pattern = pattern;

        return Task.CompletedTask;
    }
    
    private static bool ShouldTransform(OpenApiSchema schema, OpenApiSchemaTransformerContext context)
    {
        return schema.Type.HasValue && schema.Type.Value.HasFlag(JsonSchemaType.Object) &&
               schema.AdditionalProperties is { Type: not null } &&
               schema.AdditionalProperties.Type.Value.HasFlag(JsonSchemaType.String) &&
               context.JsonPropertyInfo?.AttributeProvider != null &&
               context.JsonPropertyInfo.PropertyType.IsAssignableTo(typeof(IDictionary));
    }
}
