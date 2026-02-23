using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Faque.Common.AspNet;

public static class JsonSerializerExtensions
{
    extension<T>(JsonConverter<T>? converter)
    {
        public void WriteOrSerialize(Utf8JsonWriter writer, T value, Type type, JsonSerializerOptions options)
        {
            if (converter != null)
            {
                converter.Write(writer, value, options);
            }
            else
            {
                JsonSerializer.Serialize(writer, value, type, options);
            }
        }

        public T? ReadOrDeserialize(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => converter != null
                ? converter.Read(ref reader, typeToConvert, options)
                : (T?)JsonSerializer.Deserialize(ref reader, typeToConvert, options);
    }
}
