using System.Text.Json;
using System.Text.Json.Serialization;

namespace HtmlUtilities.Validated.Standardized;

/// <summary>
/// Contents for a specialized "script" element that allows developers to control how the browser resolves module specifiers when importing JavaScript modules.
/// Reference: https://developer.mozilla.org/en-US/docs/Web/HTML/Element/script/type/importmap
/// </summary>
public sealed class ImportMap
{
    /// <summary>
    /// Replaces module specifiers in matching import statements.
    /// Effects can be overridden by a match within <see cref="Scopes"/>.
    /// </summary>
    [JsonPropertyName("imports")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<string, string>? Imports { get; set; }

    /// <summary>
    /// Provides integrity metadata for module sources.
    /// </summary>
    [JsonPropertyName("integrity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<string, string>? Integrity { get; set; }

    /// <summary>
    /// Path-specific module specifier maps.
    /// </summary>
    [JsonPropertyName("scopes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>? Scopes { get; set; }

    internal ReadOnlyMemory<byte> ToUtf8 => JsonSerializer.SerializeToUtf8Bytes(this, ImportMapSerializerContext.Default.ImportMap);

    /// <inheritdoc />
    public override string ToString() => JsonSerializer.Serialize(this, ImportMapSerializerContext.Default.ImportMap);
}

#if DEBUG
[JsonSourceGenerationOptions(WriteIndented = true)]
#endif
[JsonSerializable(typeof(ImportMap))]
internal sealed partial class ImportMapSerializerContext : JsonSerializerContext
{
}
