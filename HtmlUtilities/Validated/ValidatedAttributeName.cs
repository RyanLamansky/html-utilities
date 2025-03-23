using System.Text;

namespace HtmlUtilities.Validated;

/// <summary>
/// Enables pre-validation of HTML attribute names by storing only valid names.
/// </summary>
public readonly struct ValidatedAttributeName : IEquatable<ValidatedAttributeName>
{
    internal readonly ReadOnlyMemory<byte> value;

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
            this.value = writer;
        }
        finally
        {
            writer.Release();
        }
    }

    internal static void Validate(ReadOnlySpan<char> name, ref ArrayBuilder<byte> writer)
    {
        if (name.IsEmpty)
            throw new ArgumentException("name cannot be an empty string.", nameof(name));

        writer.Write(' ');
        foreach (var codePoint in CodePoint.GetEnumerable(name))
        {
            var categories = codePoint.InfraCategories;
            if ((categories & CodePointInfraCategory.AsciiWhitespace) == 0 && (categories & (CodePointInfraCategory.Surrogate | CodePointInfraCategory.Control)) != 0)
                throw new ArgumentException($"name has an invalid character, '{(char)codePoint.Value}'.", nameof(name));

            switch (codePoint.Value)
            {
                case '&':
                    writer.Write("&amp;"u8);
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
    /// Creates a new <see cref="ValidatedAttributeName"/> value from the provided name.
    /// </summary>
    /// <param name="name">The UTF-8 name to validate.</param>
    /// <exception cref="ArgumentException"><paramref name="name"/> is zero-length or contains invalid characters.</exception>
    public ValidatedAttributeName(ReadOnlySpan<byte> name)
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
        if (name.IsEmpty)
            throw new ArgumentException("name cannot be an empty string.", nameof(name));

        writer.Write(' ');
        foreach (var codePoint in CodePoint.GetEnumerable(name))
        {
            var categories = codePoint.InfraCategories;
            if ((categories & CodePointInfraCategory.AsciiWhitespace) == 0 && (categories & (CodePointInfraCategory.Surrogate | CodePointInfraCategory.Control)) != 0)
                throw new ArgumentException($"name has an invalid character, '{(char)codePoint.Value}'.", nameof(name));

            switch (codePoint.Value)
            {
                case '&':
                    writer.Write("&amp;"u8);
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
    public override string ToString() => Encoding.UTF8.GetString(value);

    /// <inheritdoc/>
    public override int GetHashCode() => this.value.GetContentHashCode();

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ValidatedAttributeName value && Equals(value);

    /// <inheritdoc/>
    public bool Equals(ValidatedAttributeName other) => this.value.ContentsEqual(other.value);

    /// <summary>
    /// Determines whether two instances have the same contents.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="right">The right side of the comparison.</param>
    /// <returns>True if their contents match, otherwise false.</returns>
    public static bool operator ==(ValidatedAttributeName left, ValidatedAttributeName right) => left.Equals(right);

    /// <summary>
    /// Determines whether two instances have the same contents.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="right">The right side of the comparison.</param>
    /// <returns>False if their contents match, otherwise true.</returns>
    public static bool operator !=(ValidatedAttributeName left, ValidatedAttributeName right) => !(left == right);
}
