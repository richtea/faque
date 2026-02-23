using System.Text.Json.Serialization;

namespace Faque.Common.AspNet;

/// <summary>
/// Skips the dictionary key policy for dictionaries decorated with this attribute.
/// </summary>
/// <remarks>
/// This attribute should be added to properties or fields decorated with a string key. Its purpose is to enable bypassing
/// of dictionary key policy for specific dictionaries.
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class SkipDictionaryKeyPolicyAttribute : JsonConverterAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SkipDictionaryKeyPolicyAttribute" /> class.
    /// </summary>
    public SkipDictionaryKeyPolicyAttribute()
        : base(typeof(SkipDictionaryKeyPolicyJsonConverter))
    {
    }
}
