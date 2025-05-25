using System.Text;

namespace HtmlUtilities.Validated;

/// <summary>
/// Enables pre-validation of script elements.
/// </summary>
public readonly struct ValidatedScript : IEquatable<ValidatedScript>
{
    internal readonly ReadOnlyMemory<byte> value;

    private ValidatedScript(ReadOnlyMemory<byte> value) => this.value = value;

    /// <summary>
    /// Creates a validated script element that uses the `src` attribute to link to an external file.
    /// </summary>
    /// <param name="source">The "src" attribute.</param>
    /// <param name="attributes">Optionally, other attributes for the script element.</param>
    /// <returns>The validated script.</returns>
    public static ValidatedScript ForFileSource(ReadOnlySpan<char> source, params ReadOnlySpan<ValidatedAttribute> attributes)
    {
        var writer = new ArrayBuilder<byte>(source.Length);
        try
        {
            var src = new ValidatedAttribute("src", source);
            writer.Write(src.value);

            foreach (var attribute in attributes)
                writer.Write(attribute.value);

            writer.Write('>');

            return new ValidatedScript(writer);
        }
        finally
        {
            writer.Release();
        }
    }

    /// <summary>
    /// Creates a validated script element that uses inline content.
    /// </summary>
    /// <returns>The script to validate.</returns>
    /// <exception cref="ArgumentException"><paramref name="script"/> contains a potentially invalid character sequence.</exception>
    public static ValidatedScript ForInlineSource(ReadOnlySpan<char> script, params ReadOnlySpan<ValidatedAttribute> attributes)
    {
        var writer = new ArrayBuilder<byte>(script.Length);
        try
        {
            foreach (var attribute in attributes)
                writer.Write(attribute.value);

            writer.Write('>');

            Validate(ref writer, script);
            return new ValidatedScript(writer);
        }
        finally
        {
            writer.Release();
        }
    }

    /// <summary>
    /// Creates a validated script element that uses inline content.
    /// </summary>
    /// <returns>The UTF-8 script to validate.</returns>
    /// <exception cref="ArgumentException"><paramref name="script"/> contains a potentially invalid character sequence.</exception>
    public static ValidatedScript ForInlineSource(ReadOnlySpan<byte> script, params ReadOnlySpan<ValidatedAttribute> attributes)
    {
        var writer = new ArrayBuilder<byte>(script.Length);
        try
        {
            foreach (var attribute in attributes)
                writer.Write(attribute.value);

            writer.Write('>');

            Validate(ref writer, script);
            return new ValidatedScript(writer);
        }
        finally
        {
            writer.Release();
        }
    }

    /// <summary>
    /// Creates a validated script element that uses inline content from a <see cref="Standardized.ImportMap"/>.
    /// </summary>
    /// <returns>The validated script.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="importMap"/> cannot be null.</exception>
    /// <exception cref="ArgumentException"><paramref name="importMap"/> contains a potentially invalid character sequence.</exception>
    public static ValidatedScript ForInlineSource(Standardized.ImportMap importMap, params ReadOnlySpan<ValidatedAttribute> attributes)
    {
        ArgumentNullException.ThrowIfNull(importMap);

        // Since importMap is guaranteed to be JSON-based, a fast path may be possible.
        return ForInlineSource(importMap.ToUtf8.Span, attributes);
    }

    internal static void Validate(ref ArrayBuilder<byte> writer, ReadOnlySpan<char> script)
    {
        // See https://html.spec.whatwg.org/#restrictions-for-contents-of-script-elements for the official rules.
        // It's technically possible for all risky scenarios to be corrected automatically, but that would require a fully-featured JavaScript parser.

        var temp = new string(script); // TODO: Optimal validation would operate in a single pass without a temporary string.
        if (temp.Contains("<!--", StringComparison.OrdinalIgnoreCase) || temp.Contains("<script", StringComparison.OrdinalIgnoreCase) || temp.Contains("</script", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("script contains a potentially invalid character sequence.", nameof(script));

        foreach (var c in RuneSmith.GetEnumerable(script))
            c.WriteUtf8To(ref writer);
    }

    internal static void Validate(ref ArrayBuilder<byte> writer, ReadOnlySpan<byte> script)
    {
        // See https://html.spec.whatwg.org/#restrictions-for-contents-of-script-elements for the official rules.
        // It's technically possible for all risky scenarios to be corrected automatically, but that would require a fully-featured JavaScript parser.

        var temp = new string(Encoding.UTF8.GetString(script)); // TODO: Optimal validation would operate in a single pass without a temporary string.
        if (temp.Contains("<!--", StringComparison.OrdinalIgnoreCase) || temp.Contains("<script", StringComparison.OrdinalIgnoreCase) || temp.Contains("</script", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("script contains a potentially invalid character sequence.", nameof(script));

        foreach (var c in RuneSmith.GetEnumerable(script))
            c.WriteUtf8To(ref writer);
    }

    /// <summary>
    /// Returns the script element in string form.
    /// </summary>
    /// <returns>A string representation of the script element</returns>
    public override string ToString() => $"<script{Encoding.UTF8.GetString(value)}</script>";

    /// <inheritdoc/>
    public override int GetHashCode() => this.value.GetContentHashCode();

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ValidatedScript value && Equals(value);

    /// <inheritdoc/>
    public bool Equals(ValidatedScript other) => this.value.ContentsEqual(other.value);

    /// <summary>
    /// Determines whether two instances have the same contents.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="right">The right side of the comparison.</param>
    /// <returns>True if their contents match, otherwise false.</returns>
    public static bool operator ==(ValidatedScript left, ValidatedScript right) => left.Equals(right);

    /// <summary>
    /// Determines whether two instances have the same contents.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="right">The right side of the comparison.</param>
    /// <returns>False if their contents match, otherwise true.</returns>
    public static bool operator !=(ValidatedScript left, ValidatedScript right) => !(left == right);
}
