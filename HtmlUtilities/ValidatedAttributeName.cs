using System.Text;

namespace HtmlUtilities;

/// <summary>
/// Enables pre-validation of HTML attribute names by storing only valid names.
/// </summary>
public readonly struct ValidatedAttributeName
{
    internal readonly byte[] value;

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeName"/> value from the provided name.
    /// </summary>
    /// <param name="name">The name to validate.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> cannot be null.</exception>
    /// <exception cref="ArgumentException"><paramref name="name"/> is zero-length or contains invalid characters.</exception>
    public ValidatedAttributeName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        if (name.Length == 0)
            throw new ArgumentException("name cannot be an empty string.", nameof(name));

        this.value = CodePoint.EncodeUtf8(Validate(CodePoint.DecodeUtf16(name)).Prepend(' ')).ToArray();
    }

    private static IEnumerable<CodePoint> Validate(IEnumerable<CodePoint> name)
    {
        // https://html.spec.whatwg.org/#attributes-2

        foreach (var cp in name)
        {
            var categories = cp.InfraCategories;
            const CodePointInfraCategory invalidCategories = CodePointInfraCategory.NonCharacter | CodePointInfraCategory.Control;
            if ((categories & invalidCategories) != 0)
                throw new ArgumentException($"name has an invalid character, code point {cp.Value:x2}.", nameof(name));

            switch (cp.Value)
            {
                default:
                    yield return cp;
                    continue;
                case '&':
                    yield return '&';
                    yield return 'a';
                    yield return 'm';
                    yield return 'p';
                    yield return ';';
                    continue;

                // Specific characters
                case ' ':
                case '"':
                case '\'':
                case '>':
                case '/':
                case '=':
                    throw new ArgumentException($"name has an invalid character, '{(char)cp.Value}'.", nameof(name));
            }
        }
    }

    /// <summary>
    /// Converts the validated name to a string.
    /// </summary>
    /// <returns>The string representation of the validated name.</returns>
    public override string ToString() => Encoding.UTF8.GetString(this.value);
}
