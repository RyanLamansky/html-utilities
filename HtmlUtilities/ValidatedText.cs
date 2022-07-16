using System.Text;

namespace HtmlUtilities;

/// <summary>
/// A pre-validated and formatted block of text ready to be written.
/// </summary>
public readonly struct ValidatedText
{
    internal readonly byte[]? value;

    /// <summary>
    /// Creates a new <see cref="ValidatedText"/> with the provided content.
    /// </summary>
    /// <param name="text">The text to use.</param>
    /// <remarks>Characters are escaped if needed. Invalid characters are skipped.</remarks>
    public ValidatedText(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            this.value = null;
            return;
        }

        this.value = CodePoint.EncodeUtf8(Escape(text)).ToArray();
    }

    private static IEnumerable<CodePoint> Escape(string text)
    {
        foreach (var codePoint in CodePoint.DecodeUtf16(text))
        {
            var categories = codePoint.InfraCategories;
            if ((categories & CodePointInfraCategory.AsciiWhitespace) == 0 && (categories & (CodePointInfraCategory.Surrogate | CodePointInfraCategory.Control)) != 0)
                continue;

            switch (codePoint.Value)
            {
                case '&':
                    yield return '&';
                    yield return 'a';
                    yield return 'm';
                    yield return 'p';
                    yield return ';';
                    continue;
                case '<':
                    yield return '&';
                    yield return 'l';
                    yield return 't';
                    yield return ';';
                    continue;
            }

            yield return codePoint;
        }
        yield break;
    }

    /// <summary>
    /// Returns a string of this text as it would be written.
    /// </summary>
    /// <returns>A string representation of this value.</returns>
    public override string ToString() => value is null ? "" : Encoding.UTF8.GetString(value);
}
