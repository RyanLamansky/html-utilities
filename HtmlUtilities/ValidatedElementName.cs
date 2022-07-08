using System.Text;

namespace HtmlUtilities;

/// <summary>
/// Enables pre-validation of HTML element names by storing only valid names.
/// </summary>
public readonly struct ValidatedElementName
{
    internal readonly byte[] value;

    /// <summary>
    /// Creates a new <see cref="ValidatedElementName"/> value from the provided name.
    /// </summary>
    /// <param name="name">The UTF-8 bytes of the name to validate.</param>
    /// <exception cref="ArgumentException">The element name is not valid.</exception>
    public ValidatedElementName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        this.value = CodePoint.EncodeUtf8(Validate(CodePoint.DecodeUtf16(name))).ToArray();
    }

    private static IEnumerable<CodePoint> Validate(IEnumerable<CodePoint> name)
    {
        // https://html.spec.whatwg.org/#syntax-tag-name
        // Summary of above:
        // - Must be at least one charcter
        // - First character must be ASCII alpha
        // - Rest must be ASCII alpha or ASCII digit

        using var enumerator = name.GetEnumerator();

        if (!enumerator.MoveNext())
            throw new ArgumentException("Element name cannot be an empty string.", nameof(name));

        if (enumerator.Current.Value is not (>= 'a' and <= 'z' or >= 'A' and <= 'Z'))
            throw new ArgumentException("Element names must have an ASCII alpha as the first character.", nameof(name));

        yield return enumerator.Current;

        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Value is not (>= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9'))
                throw new ArgumentException("Element names cannot have characters outside the range of ASCII alpha or digits.", nameof(name));

            yield return enumerator.Current;
        }
    }

    /// <summary>
    /// Converts the validated name to a string.
    /// </summary>
    /// <returns>The string representation of the validated name.</returns>
    public override string ToString() => Encoding.UTF8.GetString(this.value);
}
