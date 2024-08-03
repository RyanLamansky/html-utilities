using System.Text;

namespace HtmlUtilities.Validated;

/// <summary>
/// Enables pre-validation of script elements.
/// </summary>
public readonly struct ValidatedScript
{
    internal readonly ReadOnlyMemory<byte> value;

    private ValidatedScript(ReadOnlyMemory<byte> value) => this.value = value;

    /// <summary>
    /// Creates a validated script element that uses the `src` attribute to link to an external file.
    /// </summary>
    /// <param name="source">The "src" attribute.</param>
    /// <param name="attributes">Optionally, other attributes for the script element.</param>
    /// <returns>The validated script.</returns>
    public static ValidatedScript ForFileSource(ReadOnlySpan<char> source, params ValidatedAttribute[]? attributes)
    {
        var writer = new ArrayBuilder<byte>(source.Length);
        try
        {
            foreach (var attribute in (attributes ?? Enumerable.Empty<ValidatedAttribute>()).Prepend(new ValidatedAttribute("src", source)))
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
    /// <returns>The validated script.</returns>
    /// <exception cref="ArgumentException"><paramref name="script"/>  contains a potentially invalid character sequence.</exception>
    public static ValidatedScript ForInlineSource(ReadOnlySpan<char> script, params ValidatedAttribute[]? attributes)
    {
        var writer = new ArrayBuilder<byte>(script.Length);
        try
        {
            foreach (var attribute in attributes ?? [])
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

    internal static void Validate(ref ArrayBuilder<byte> writer, ReadOnlySpan<char> script)
    {
        // See https://html.spec.whatwg.org/#restrictions-for-contents-of-script-elements for the official rules.
        // It's technically possible for all risky scenarios to be corrected automatically, but that would require a fully-featured JavaScript parser.

        var temp = new string(script); // Optimal validation would operate in a single pass without a temporary string.
        if (temp.Contains("<!--") || temp.Contains("<script", StringComparison.OrdinalIgnoreCase) || temp.Contains("</script", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("script contains a potentially invalid character sequence.", nameof(script));

        foreach (var c in CodePoint.GetEnumerable(script))
            c.WriteUtf8To(ref writer);
    }

    /// <summary>
    /// Returns the script element in string form.
    /// </summary>
    /// <returns>A string representation of the script element</returns>
    public override string ToString() => $"<script{Encoding.UTF8.GetString(value)}</script>";
}
