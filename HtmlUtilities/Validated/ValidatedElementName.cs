using System.Text;

namespace HtmlUtilities.Validated;

/// <summary>
/// Enables pre-validation of HTML element names by storing only valid names.
/// </summary>
public readonly struct ValidatedElementName : IEquatable<ValidatedElementName>
{
    internal readonly ReadOnlyMemory<byte> value;

    /// <summary>
    /// Creates a new <see cref="ValidatedElementName"/> value from the provided name.
    /// </summary>
    /// <param name="name">The UTF-8 bytes of the name to validate.</param>
    /// <exception cref="ArgumentException">The element name is not valid.</exception>
    public ValidatedElementName(ReadOnlySpan<char> name)
    {
        var writer = new ArrayBuilder<byte>(name.Length);
        try
        {
            Validate(name, ref writer);

            this.value = writer;
        }
        finally
        {
            writer.Release();
        }
    }

    internal static void Validate(ReadOnlySpan<char> name, ref ArrayBuilder<byte> writer)
    {
        // https://html.spec.whatwg.org/#syntax-tag-name
        // Summary of above:
        // - Must be at least one character
        // - First character must be ASCII alpha
        // - Rest must be ASCII alpha or ASCII digit

        if (name.IsEmpty)
            throw new ArgumentException("name cannot be an empty string.", nameof(name));

        if (name.Length == 6 && name[0] is 'S' or 's' && name[1] is 'C' or 'c' && name[2] is 'R' or 'r' && name[3] is 'I' or 'i' && name[4] is 'P' or 'p' && name[5] is 'T' or 't')
            throw new ArgumentException("Use ValidatedScript or WriteScript for script elements.", nameof(name));

        var enumerator = RuneSmith.GetEnumerable(name).GetEnumerator();

        if (!enumerator.MoveNext())
            throw new ArgumentException("Element name cannot be an empty string.", nameof(name));

        var codePoint = enumerator.Current;

        if ((enumerator.Current.InfraCategories() & CodePointInfraCategory.AsciiAlpha) == 0)
            throw new ArgumentException("Element names must have an ASCII alpha as the first character.", nameof(name));

        codePoint.WriteUtf8To(ref writer);

        while (enumerator.MoveNext())
        {
            codePoint = enumerator.Current;
            if ((codePoint.InfraCategories() & CodePointInfraCategory.AsciiAlphanumeric) == 0)
                throw new ArgumentException("Element names cannot have characters outside the range of ASCII alpha or digits.", nameof(name));

            codePoint.WriteUtf8To(ref writer);
        }
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedElementName"/> value from the provided name.
    /// </summary>
    /// <param name="name">The UTF-8 bytes of the name to validate.</param>
    /// <exception cref="ArgumentException">The element name is not valid.</exception>
    public ValidatedElementName(ReadOnlySpan<byte> name)
    {
        var writer = new ArrayBuilder<byte>(name.Length);
        try
        {
            Validate(name, ref writer);

            this.value = writer;
        }
        finally
        {
            writer.Release();
        }
    }

    internal static void Validate(ReadOnlySpan<byte> name, ref ArrayBuilder<byte> writer)
    {
        // https://html.spec.whatwg.org/#syntax-tag-name
        // Summary of above:
        // - Must be at least one character
        // - First character must be ASCII alpha
        // - Rest must be ASCII alpha or ASCII digit

        if (name.IsEmpty)
            throw new ArgumentException("name cannot be an empty string.", nameof(name));

        if (name.Length == 6 && (char)name[0] is 'S' or 's' && (char)name[1] is 'C' or 'c' && (char)name[2] is 'R' or 'r' && (char)name[3] is 'I' or 'i' && (char)name[4] is 'P' or 'p' && (char)name[5] is 'T' or 't')
            throw new ArgumentException("Use ValidatedScript or WriteScript for script elements.", nameof(name));

        var enumerator = RuneSmith.GetEnumerable(name).GetEnumerator();

        if (!enumerator.MoveNext())
            throw new ArgumentException("Element name cannot be an empty string.", nameof(name));

        var codePoint = enumerator.Current;

        if ((enumerator.Current.InfraCategories() & CodePointInfraCategory.AsciiAlpha) == 0)
            throw new ArgumentException("Element names must have an ASCII alpha as the first character.", nameof(name));

        codePoint.WriteUtf8To(ref writer);

        while (enumerator.MoveNext())
        {
            codePoint = enumerator.Current;
            if ((codePoint.InfraCategories() & CodePointInfraCategory.AsciiAlphanumeric) == 0)
                throw new ArgumentException("Element names cannot have characters outside the range of ASCII alpha or digits.", nameof(name));

            codePoint.WriteUtf8To(ref writer);
        }
    }

    /// <summary>
    /// Converts the validated name to a string.
    /// </summary>
    /// <returns>The string representation of the validated name.</returns>
    public override string ToString() => Encoding.UTF8.GetString(value);

    /// <inheritdoc/>
    public override int GetHashCode() => this.value.GetContentHashCode();

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ValidatedElementName value && Equals(value);

    /// <inheritdoc/>
    public bool Equals(ValidatedElementName other) => this.value.ContentsEqual(other.value);

    /// <summary>
    /// Determines whether two instances have the same content.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="right">The right side of the comparison.</param>
    /// <returns>True if their contents match, otherwise false.</returns>
    public static bool operator ==(ValidatedElementName left, ValidatedElementName right) => left.Equals(right);

    /// <summary>
    /// Determines whether two instances have the same contents.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="right">The right side of the comparison.</param>
    /// <returns>False if their contents match, otherwise true.</returns>
    public static bool operator !=(ValidatedElementName left, ValidatedElementName right) => !(left == right);
}
