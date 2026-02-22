using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Faque.Common.AspNet;

/// <summary>
/// A JSON converter that skips the dictionary key policy for dictionaries with string keys.
/// </summary>
/// <remarks>
/// This converter should not be added to the global JSON converters collection. Its purpose is to enable bypassing of
/// dictionary key policy for specific dictionaries. Trigger it by decorating a property with
/// <see cref="SkipDictionaryKeyPolicyAttribute" />.
/// </remarks>
public class SkipDictionaryKeyPolicyJsonConverter : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType || (typeToConvert.GetGenericTypeDefinition() != typeof(Dictionary<,>)))
        {
            return false;
        }

        var parameters = GetDictionaryKeyValueTypes(typeToConvert);

        if (parameters == null)
        {
            return false;
        }

        return (parameters[0] == typeof(string)) &&
            (typeToConvert.GetConstructor(Type.EmptyTypes) != null);
    }

    /// <inheritdoc />
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var parameters = GetDictionaryKeyValueTypes(typeToConvert);

        if (parameters == null)
        {
            return null;
        }

        var converter = (JsonConverter?)Activator.CreateInstance(
            typeof(SkipKeyPolicyConverterInner<,>).MakeGenericType(typeToConvert, parameters[1]),
            BindingFlags.Instance | BindingFlags.Public,
            null,
            null,
            null);

        return converter;
    }

    private static Type[]? GetDictionaryKeyValueTypes(Type typeToConvert)
    {
        // Get the type's implemented interfaces
        var interfaces = typeToConvert.IsInterface
            ? [..typeToConvert.GetInterfaces(), typeToConvert]
            : typeToConvert.GetInterfaces();

        var dict = interfaces
            .FirstOrDefault(t => t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(IDictionary<,>)));

        return dict?.GetGenericArguments();
    }

    private class SkipKeyPolicyConverterInner<TDictionary, TValue> : JsonConverter<TDictionary>
        where TDictionary : IDictionary<string, TValue>, new()
    {
        private readonly Type _valueType = typeof(TValue);

        public override TDictionary? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default; // Or throw an exception if you prefer.
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var dictionary = new TDictionary();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return dictionary;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var key = reader.GetString();
                reader.Read();

                if (key == null)
                {
                    throw new JsonException();
                }

                var value = (TValue?)JsonSerializer.Deserialize(ref reader, _valueType, options);
                if ((value == null) && !IsNullable(_valueType))
                {
                    throw new JsonException($"Unexpected null value for non-nullable type '{_valueType}'.");
                }

                dictionary.Add(key, value!);
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, TDictionary dictionary, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach (var pair in dictionary)
            {
                writer.WritePropertyName(pair.Key);
                JsonSerializer.Serialize(writer, pair.Value, _valueType, options);
            }

            writer.WriteEndObject();
        }

        private static bool IsNullable(Type type)
        {
            return !type.IsValueType || (Nullable.GetUnderlyingType(type) != null);
        }
    }
}
