using System.Text;

namespace HtmlUtilities.Validated;

/// <summary>
/// A pre-validated and formatted block of text ready to be written.
/// </summary>
public readonly struct ValidatedText : IEquatable<ValidatedText>
{
    internal readonly ReadOnlyMemory<byte> value;

    /// <summary>
    /// Creates a new <see cref="ValidatedText"/> with the provided content.
    /// </summary>
    /// <param name="text">The text to use.</param>
    /// <remarks>Characters are escaped if needed. Invalid characters are skipped.</remarks>
    public ValidatedText(ReadOnlySpan<char> text)
    {
        var writer = new ArrayBuilder<byte>(text.Length);
        try
        {
            Validate(text, ref writer);

            this.value = writer;
        }
        finally
        {
            writer.Release();
        }
    }

    internal ValidatedText(ReadOnlyMemory<byte> value) => this.value = value;

    internal static void Validate(ReadOnlySpan<char> text, ref ArrayBuilder<byte> writer)
    {
        if (text.IsEmpty)
            return;

        foreach (var codePoint in CodePoint.GetEnumerable(text))
        {
            var categories = codePoint.InfraCategories;
            if ((categories & CodePointInfraCategory.AsciiWhitespace) == 0 &&
                (categories & (CodePointInfraCategory.Surrogate | CodePointInfraCategory.Control | CodePointInfraCategory.NonCharacter)) != 0)
                continue;

            switch (codePoint.Value)
            {
                case '&':
                    writer.Write("&amp;"u8);
                    continue;
                case '<':
                    writer.Write("&lt;"u8);
                    continue;
            }

            codePoint.WriteUtf8To(ref writer);
        }
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedText"/> with the provided content.
    /// </summary>
    /// <param name="text">The UTF-8 text to use.</param>
    /// <remarks>Characters are escaped if needed. Invalid characters are skipped.</remarks>
    public ValidatedText(ReadOnlySpan<byte> text)
    {
        var writer = new ArrayBuilder<byte>(text.Length);
        try
        {
            Validate(text, ref writer);

            this.value = writer;
        }
        finally
        {
            writer.Release();
        }
    }

    internal static void Validate(ReadOnlySpan<byte> text, ref ArrayBuilder<byte> writer)
    {
        if (text.IsEmpty)
            return;

        foreach (var codePoint in CodePoint.GetEnumerable(text))
        {
            var categories = codePoint.InfraCategories;
            if ((categories & CodePointInfraCategory.AsciiWhitespace) == 0 &&
                (categories & (CodePointInfraCategory.Surrogate | CodePointInfraCategory.Control | CodePointInfraCategory.NonCharacter)) != 0)
                continue;

            switch (codePoint.Value)
            {
                case '&':
                    writer.Write("&amp;"u8);
                    continue;
                case '<':
                    writer.Write("&lt;"u8);
                    continue;
            }

            codePoint.WriteUtf8To(ref writer);
        }
    }

    /// <summary>
    /// When true, the content is empty.
    /// </summary>
    public bool IsEmpty => this.value.IsEmpty;

    /// <summary>
    /// Returns a string of this text as it would be written.
    /// </summary>
    /// <returns>A string representation of this value.</returns>
    public override string ToString() => Encoding.UTF8.GetString(value);

    /// <inheritdoc/>
    public override int GetHashCode() => this.value.GetContentHashCode();

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ValidatedText value && Equals(value);

    /// <inheritdoc/>
    public bool Equals(ValidatedText other) => this.value.ContentsEqual(other.value);

    /// <summary>
    /// Determines whether two instances have the same content.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="right">The right side of the comparison.</param>
    /// <returns>True if their contents match, otherwise false.</returns>
    public static bool operator ==(ValidatedText left, ValidatedText right) => left.Equals(right);

    /// <summary>
    /// Determines whether two instances have the same contents.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="right">The right side of the comparison.</param>
    /// <returns>False if their contents match, otherwise true.</returns>
    public static bool operator !=(ValidatedText left, ValidatedText right) => !(left == right);
}
