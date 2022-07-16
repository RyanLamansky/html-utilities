using System.Text;

namespace HtmlUtilities;

/// <summary>
/// A pre-validated and formatted attribute value ready to be written.
/// </summary>
public readonly struct ValidatedAttributeValue
{
    internal readonly byte[]? value;

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeValue"/> from the provided string.
    /// </summary>
    /// <param name="value">The value to prepare as an attribute.</param>
    public ValidatedAttributeValue(string? value)
    {
        // See https://html.spec.whatwg.org/#attributes-2 for reference.

        if (value is null)
        {
            this.value = null;
            return;
        }

        if (value.Length == 0)
        {
            this.value = new[] { (byte)'=', (byte)'"', (byte)'"' };
            return;
        }

        this.value = CodePoint.EncodeUtf8(SelectEmitter(value)).ToArray();
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeValue"/> from the provided number.
    /// </summary>
    /// <param name="value">The value to prepare as an attribute.</param>
    public ValidatedAttributeValue(int value)
    {
        this.value = ToUtf8Array(value);
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeValue"/> from the provided number.
    /// </summary>
    /// <param name="value">The value to prepare as an attribute.</param>
    public ValidatedAttributeValue(int? value)
    {
        if (value is null)
        {
            this.value = Array.Empty<byte>();
            return;
        }

        this.value = ToUtf8Array(value.GetValueOrDefault());
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeValue"/> from the provided number.
    /// </summary>
    /// <param name="value">The value to prepare as an attribute.</param>
    public ValidatedAttributeValue(uint value)
    {
        this.value = ToUtf8Array(value);
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeValue"/> from the provided number.
    /// </summary>
    /// <param name="value">The value to prepare as an attribute.</param>
    public ValidatedAttributeValue(uint? value)
    {
        if (value is null)
        {
            this.value = Array.Empty<byte>();
            return;
        }

        this.value = ToUtf8Array(value.GetValueOrDefault());
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeValue"/> from the provided number.
    /// </summary>
    /// <param name="value">The value to prepare as an attribute.</param>
    public ValidatedAttributeValue(long value)
    {
        this.value = ToUtf8Array(value);
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeValue"/> from the provided number.
    /// </summary>
    /// <param name="value">The value to prepare as an attribute.</param>
    public ValidatedAttributeValue(long? value)
    {
        if (value is null)
        {
            this.value = Array.Empty<byte>();
            return;
        }

        this.value = ToUtf8Array(value.GetValueOrDefault());
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeValue"/> from the provided number.
    /// </summary>
    /// <param name="value">The value to prepare as an attribute.</param>
    public ValidatedAttributeValue(ulong value)
    {
        this.value = ToUtf8Array(value);
    }

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeValue"/> from the provided number.
    /// </summary>
    /// <param name="value">The value to prepare as an attribute.</param>
    public ValidatedAttributeValue(ulong? value)
    {
        if (value is null)
        {
            this.value = Array.Empty<byte>();
            return;
        }

        this.value = ToUtf8Array(value.GetValueOrDefault());
    }

    private static byte[] ToUtf8Array(long value)
    {
        if (value >= 0)
            return ToUtf8Array((ulong)value);

        var result = new byte[value switch
        {
            > -10 => 3,
            > -100 => 4,
            > -1000 => 5,
            > -10000 => 6,
            > -100000 => 7,
            > -1000000 => 8,
            > -10000000 => 9,
            > -100000000 => 10,
            > -1000000000 => 11,
            > -10000000000 => 12,
            > -100000000000 => 13,
            > -1000000000000 => 14,
            > -10000000000000 => 15,
            > -100000000000000 => 16,
            > -1000000000000000 => 17,
            > -10000000000000000 => 18,
            > -100000000000000000 => 19,
            > -1000000000000000000 => 20,
            _ => 21,
        }];

        for (var i = 2; i < result.Length; i++)
        {
            result[i] = (byte)('0' + Math.Abs((int)(value % 10)));
            value /= 10;
        }

        Array.Reverse(result, 2, result.Length - 2);
        result[0] = (byte)'=';
        result[1] = (byte)'-';

        return result;
    }

    private static byte[] ToUtf8Array(ulong value)
    {
        var result = new byte[value switch
        {
            < 10 => 2,
            < 100 => 3,
            < 1000 => 4,
            < 10000 => 5,
            < 100000 => 6,
            < 1000000 => 7,
            < 10000000 => 8,
            < 100000000 => 9,
            < 1000000000 => 10,
            < 10000000000 => 11,
            < 100000000000 => 12,
            < 1000000000000 => 13,
            < 10000000000000 => 14,
            < 100000000000000 => 15,
            < 1000000000000000 => 16,
            < 10000000000000000 => 17,
            < 100000000000000000 => 18,
            < 1000000000000000000 => 19,
            < 10000000000000000000 => 20,
            _ => 21
        }];

        for (var i = 1; i < result.Length; i++)
        {
            result[i] = (byte)('0' + (int)(value % 10));
            value /= 10;
        }

        Array.Reverse(result, 1, result.Length - 1);
        result[0] = (byte)'=';

        return result;
    }

    private static IEnumerable<CodePoint> SelectEmitter(string value)
    {
        foreach (var codePoint in CodePoint.DecodeUtf16(value))
        {
            switch (codePoint.Value)
            {
                // Other characters that require quoting
                case '"':
                case '\'':
                case '=':
                case '<':
                case '>':
                case '`':
                    break;
                default:
                    if ((codePoint.InfraCategories & CodePointInfraCategory.AsciiWhitespace) != 0)
                        break;

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
    public override string ToString() => value is null ? "" : Encoding.UTF8.GetString(value);
}
