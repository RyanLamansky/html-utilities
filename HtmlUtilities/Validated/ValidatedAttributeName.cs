using System.Text;

namespace HtmlUtilities.Validated;

/// <summary>
/// Enables pre-validation of HTML attribute names by storing only valid names.
/// </summary>
public readonly struct ValidatedAttributeName
{
    internal readonly byte[]? value;

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeName"/> value from the provided name.
    /// </summary>
    /// <param name="name">The name to validate.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is zero-length or contains invalid characters.</exception>
    public ValidatedAttributeName(ReadOnlySpan<char> name)
    {
        var writer = new ArrayBuilder<byte>(name.Length);
        try
        {
            Validate(name, ref writer);
            this.value = writer.ToArray();
        }
        finally
        {
            writer.Release();
        }
    }

    internal static void Validate(ReadOnlySpan<char> name, ref ArrayBuilder<byte> writer)
    {
        if (name.Length == 0)
            throw new ArgumentException("name cannot be an empty string.", nameof(name));

        writer.Write((byte)' ');
        foreach (var codePoint in CodePoint.GetEnumerable(name))
        {
            var categories = codePoint.InfraCategories;
            if ((categories & CodePointInfraCategory.AsciiWhitespace) == 0 && (categories & (CodePointInfraCategory.Surrogate | CodePointInfraCategory.Control)) != 0)
                continue;

            switch (codePoint.Value)
            {
                case '&':
                    writer.Write(NamedCharacterReferences.Ampersand);
                    continue;

                // Specific characters
                case ' ':
                case '"':
                case '\'':
                case '>':
                case '/':
                case '=':
                    throw new ArgumentException($"name has an invalid character, '{(char)codePoint.Value}'.", nameof(name));
            }

            codePoint.WriteUtf8To(ref writer);
        }
    }

    /// <summary>
    /// Converts the validated name to a string.
    /// </summary>
    /// <returns>The string representation of the validated name.</returns>
    /// <exception cref="InvalidOperationException">This ValidatedAttributeName was never initialized.</exception>
    public override string ToString() => Encoding.UTF8.GetString(value ?? throw new InvalidOperationException("This ValidatedAttributeName was never initialized."));

    /// <summary>
    /// Creates a new <see cref="ValidatedAttribute"/> from the provided validated name and no value.
    /// </summary>
    /// <param name="name">A validated name.</param>
    public static implicit operator ValidatedAttribute(ValidatedAttributeName name) => new(name);
}
