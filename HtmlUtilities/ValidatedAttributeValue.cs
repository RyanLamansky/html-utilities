using System.Text;

namespace HtmlUtilities;

/// <summary>
/// A pre-validated and formatted attribute value ready to be written.
/// </summary>
public readonly struct ValidatedAttributeValue
{
    internal readonly byte[] value;

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeValue"/> from the provided string.
    /// </summary>
    /// <param name="value">The value to prepare as an attribute.</param>
    public ValidatedAttributeValue(string? value)
    {
        // See https://html.spec.whatwg.org/#attributes-2 for reference.

        if (value is null)
        {
            this.value = Array.Empty<byte>();
            return;
        }

        if (value.Length == 0)
        {
            this.value = new[] { (byte)'=', (byte)'"', (byte)'"' };
            return;
        }

        this.value = CodePoint.EncodeUtf8(SelectEmitter(value)).ToArray();
    }

    private static IEnumerable<CodePoint> SelectEmitter(IEnumerable<char> value)
    {
        foreach (var codePoint in CodePoint.DecodeUtf16(value))
        {
            switch (codePoint.Value)
            {
                // ASCII Whitespace
                case 0x0009:
                case 0x000A:
                case 0x000C:
                case 0x000D:
                case 0x0020:

                // Other characters that require quoting
                case '"':
                case '\'':
                case '=':
                case '<':
                case '>':
                case '`':
                    break;
                default:
                    continue;
            }

            return EmitQuoted(CodePoint.DecodeUtf16(value));
        }

        return EmitUnquoted(CodePoint.DecodeUtf16(value));
    }

    private static IEnumerable<CodePoint> EmitUnquoted(IEnumerable<CodePoint> value)
    {
        yield return '=';

        foreach (var cp in value)
        {
            switch (cp.Value)
            {
                case '&':
                    yield return '&';
                    yield return 'a';
                    yield return 'm';
                    yield return 'p';
                    yield return ';';
                    continue;
            }

            yield return cp;
        }
    }

    private static IEnumerable<CodePoint> EmitQuoted(IEnumerable<CodePoint> value)
    {
        yield return '=';
        yield return '"';

        foreach (var cp in value)
        {
            switch (cp.Value)
            {
                case '"':
                    yield return '&';
                    yield return 'q';
                    yield return 'u';
                    yield return 'o';
                    yield return 't';
                    yield return ';';
                    continue;
                case '&':
                    yield return '&';
                    yield return 'a';
                    yield return 'm';
                    yield return 'p';
                    yield return ';';
                    continue;
            }

            yield return cp;
        }

        yield return '"';
    }

    /// <summary>
    /// Returns a string of this attribute value as it would be written.
    /// </summary>
    /// <returns>A string representation of this value.</returns>
    public override string ToString() => Encoding.UTF8.GetString(this.value);
}
