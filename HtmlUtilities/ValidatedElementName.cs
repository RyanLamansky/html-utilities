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
    /// <exception cref="Exception"></exception>
    public ValidatedElementName(ReadOnlySpan<byte> name)
    {
        if (!IsValid(name))
            throw new Exception();

        this.value = name.ToArray(); // Copies the source so that it can't be changed after validation.
    }

    /// <summary>
    /// Determines whether the provided--potentially custom--element name is valid acccording to HTML5 rules.
    /// </summary>
    /// <param name="name">The name to validate.</param>
    /// <returns>True if <paramref name="name"/> is a valid element name, otherwise false.</returns>
    public static bool IsValid(ReadOnlySpan<byte> name)
    {
        // https://html.spec.whatwg.org/#parsing
        // https://html.spec.whatwg.org/#syntax-start-tag
        // Summary of above:
        // - Must be at least one charcter
        // - First character must be ASCII alpha
        // - Rest must be ASCII alpha or ASCII digit

        var enumerator = name.GetEnumerator();
        if (!enumerator.MoveNext())
            return false;

        if ((char)enumerator.Current is not (>= 'a' and <= 'z' or >= 'A' and <= 'Z'))
            return false;

        while (enumerator.MoveNext())
        {
            if ((char)enumerator.Current is not (>= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9'))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Converts the validated name to a string.
    /// </summary>
    /// <returns>The string representation of the validated name.</returns>
    public override string ToString() => Encoding.UTF8.GetString(this.value);

    /// <summary>
    /// Creates a new <see cref="ValidatedElementName"/> value from the provided name.
    /// </summary>
    /// <param name="name">The UTF-8 bytes of the name to validate.</param>
    /// <exception cref="Exception"></exception>
    public static implicit operator ValidatedElementName(ReadOnlySpan<byte> name) => new(name);
}
