using System.Collections.Frozen;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Faque.Common.OpenApi;

/// <summary>
/// A schema transformer that modifies the generated OpenAPI schema for numeric types to adhere to the Principle of Least
/// Astonishment.
/// </summary>
/// <remarks>
///     <para>
///     Astonishingly, the default behavior of <c>Microsoft.AspNetCore.OpenApi</c> is to set the schema type to include
///     <c>string</c> for numeric types. See <see href="https://github.com/dotnet/aspnetcore/issues/64920">lots</see>
///     <see href="https://github.com/dotnet/aspnetcore/issues/61038">and</see>
///     <see href="https://github.com/dotnet/aspnetcore/issues/62963">lots</see>
///     <see href="https://github.com/dotnet/aspnetcore/issues/64473">and</see>
///     <see href="https://github.com/dotnet/aspnetcore/issues/64920">lots</see> of issues that demonstrate the
///     astonishment.
///     </para>
///     <para>
///     This behavior is actually quite logical, but it's not what most people expect. It's because the default value of
///     <see cref="JsonSerializerOptions.NumberHandling" /> is set to
///     <see cref="JsonNumberHandling.AllowReadingFromString" />, and <c>Microsoft.AspNetCore.OpenApi</c>
///     therefore sets the schema type to allow <c>string</c> as well as <c>number</c>.
///     </para>
///     <para>
///     The suggested workaround is to set <see cref="JsonSerializerOptions.NumberHandling" /> to
///     <see cref="JsonNumberHandling.Strict" />, but that's not always desirable. This transformer fixes the issue by
///     removing the <see cref="JsonSchemaType.String"/> flag from the schema type.
///     </para>
/// </remarks>
public class OpenApiNumberDeastonisher : IOpenApiSchemaTransformer
{
    private static readonly FrozenSet<Type> NumericTypes;

    static OpenApiNumberDeastonisher()
    {
        HashSet<Type> numericTypes =
        [
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(float),
            typeof(double),
            typeof(decimal),
        ];
        NumericTypes = numericTypes.ToFrozenSet();
    }

    /// <inheritdoc />
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (schema.Type == null)
        {
            return Task.CompletedTask;
        }

        var underlyingType = Nullable.GetUnderlyingType(context.JsonTypeInfo.Type) ?? context.JsonTypeInfo.Type;

        if (!NumericTypes.Contains(underlyingType))
        {
            return Task.CompletedTask;
        }

        schema.Type &= ~JsonSchemaType.String;
        schema.Pattern = null;

        return Task.CompletedTask;
    }
}
