using System.Text;

namespace HtmlUtilities;

/// <summary>
/// A pre-validated and formatted block of text ready to be written.
/// </summary>
public readonly struct ValidatedText
{
    private static readonly byte[] andAmp = new[] { (byte)'&', (byte)'a', (byte)'m', (byte)'p', (byte)';', };
    private static readonly byte[] andLt = new[] { (byte)'&', (byte)'l', (byte)'t', (byte)';', };

    internal readonly byte[]? value;

    /// <summary>
    /// Creates a new <see cref="ValidatedText"/> with the provided content.
    /// </summary>
    /// <param name="text">The text to use.</param>
    /// <remarks>Characters are escaped if needed. Invalid characters are skipped.</remarks>
    public ValidatedText(string? text)
        : this((ReadOnlySpan<char>)text)
    {
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedText"/> with the provided content.
    /// </summary>
    /// <param name="text">The text to use.</param>
    /// <remarks>Characters are escaped if needed. Invalid characters are skipped.</remarks>
    public ValidatedText(ReadOnlySpan<char> text)
    {
        if (text.IsEmpty)
        {
            this.value = null;
            return;
        }

        var writer = new ArrayBuilder<byte>(text.Length);
        try
        {
            foreach (var codePoint in CodePoint.GetEnumerable(text))
            {
                var categories = codePoint.InfraCategories;
                if ((categories & CodePointInfraCategory.AsciiWhitespace) == 0 && (categories & (CodePointInfraCategory.Surrogate | CodePointInfraCategory.Control)) != 0)
                    continue;

                switch (codePoint.Value)
                {
                    case '&':
                        writer.Write(andAmp);
                        continue;
                    case '<':
                        writer.Write(andLt);
                        continue;
                }

                codePoint.WriteUtf8To(ref writer);
            }

            this.value = writer.ToArray();
        }
        finally
        {
            writer.Release();
        }
    }

    /// <summary>
    /// Returns a string of this text as it would be written.
    /// </summary>
    /// <returns>A string representation of this value.</returns>
    public override string ToString() => value is null ? "" : Encoding.UTF8.GetString(value);
}
