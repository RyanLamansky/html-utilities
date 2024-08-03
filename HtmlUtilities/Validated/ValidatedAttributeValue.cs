using System.Text;

namespace HtmlUtilities.Validated;

/// <summary>
/// A pre-validated and formatted attribute value ready to be written.
/// </summary>
/// <remarks>The rules described by https://html.spec.whatwg.org/#attributes-2 are closely followed.</remarks>
public readonly struct ValidatedAttributeValue
{
    internal readonly ReadOnlyMemory<byte> value;

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeValue"/> from the provided <see cref="ReadOnlySpan{T}"/> of type <see cref="char"/>.
    /// </summary>
    /// <param name="value">The value to prepare as an attribute.</param>
    public ValidatedAttributeValue(ReadOnlySpan<char> value) => this.value = ToUtf8Array(value);

    private static ReadOnlyMemory<byte> ToUtf8Array(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty)
            return Array.Empty<byte>(); // Empty attribute syntax implicitly has a value of an empty string.

        var writer = new ArrayBuilder<byte>(value.Length);

        try
        {
            Validate(value, ref writer);
            return writer;
        }
        finally
        {
            writer.Release();
        }
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
            return;

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
            return;

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
            return;

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
            return;

        this.value = ToUtf8Array(value.GetValueOrDefault());
    }

    private static byte[] ToUtf8Array(long value)
    {
        if (value >= 0)
            return ToUtf8Array((ulong)value);

        var result = new byte[CountBytes(value)];

        ToUtf8(value, result);

        return result;
    }

    private static byte[] ToUtf8Array(ulong value)
    {
        var result = new byte[CountBytes(value)];

        ToUtf8(value, result);

        return result;
    }

    internal static int CountBytes(long value) => value switch
    {
        >= 0 => CountBytes((ulong)value),
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
    };

    internal static void ToUtf8(long value, Span<byte> result)
    {
        if (value > 0)
        {
            ToUtf8((ulong)value, result);
            return;
        }

        for (var i = result.Length - 1; i > 1; i--)
        {
            result[i] = (byte)('0' + -(value % 10));
            value /= 10;
        }

        result[0] = (byte)'=';
        result[1] = (byte)'-';
    }

    internal static int CountBytes(ulong value) => value switch
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
    };

    internal static void ToUtf8(ulong value, Span<byte> result)
    {
        for (var i = result.Length - 1; i > 0; i--)
        {
            result[i] = (byte)('0' + (int)(value % 10));
            value /= 10;
        }

        result[0] = (byte)'=';
    }

    internal static void Validate(ReadOnlySpan<char> value, ref ArrayBuilder<byte> writer)
    {
        if (value.IsEmpty)
            return;

        foreach (var codePoint in CodePoint.GetEnumerable(value))
        {
            switch (codePoint.Value)
            {
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

            EmitQuoted(value, ref writer);
            return;
        }

        EmitUnquoted(value, ref writer);
    }

    private static void EmitUnquoted(ReadOnlySpan<char> value, ref ArrayBuilder<byte> writer)
    {
        writer.Write('=');

        foreach (var codePoint in CodePoint.GetEnumerable(value))
        {
            switch (codePoint.Value)
            {
                case '&':
                    writer.Write("&amp;"u8);
                    continue;
            }

            codePoint.WriteUtf8To(ref writer);
        }
    }

    private static void EmitQuoted(ReadOnlySpan<char> value, ref ArrayBuilder<byte> writer)
    {
        writer.Write('=');
        writer.Write('"');

        foreach (var codePoint in CodePoint.GetEnumerable(value))
        {
            switch (codePoint.Value)
            {
                case '&':
                    writer.Write("&amp;"u8);
                    continue;
                case '"':
                    writer.Write("&quot;"u8);
                    continue;
            }

            codePoint.WriteUtf8To(ref writer);
        }

        writer.Write('"');
    }

    /// <summary>
    /// Returns a string of this attribute value as it would be written.
    /// </summary>
    /// <returns>A string representation of this value.</returns>
    public override string ToString() => Encoding.UTF8.GetString(value);

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeValue"/> from the provided <see cref="ReadOnlySpan{T}"/> of type <see cref="char"/>.
    /// </summary>
    /// <param name="value">The value to prepare as an attribute.</param>
    public static implicit operator ValidatedAttributeValue(ReadOnlySpan<char> value) => new(value);

    /// <summary>
    /// Creates a new <see cref="ValidatedAttributeValue"/> from the provided <see cref="string"/>.
    /// </summary>
    /// <param name="value">The value to prepare as an attribute.</param>
    public static implicit operator ValidatedAttributeValue(string? value) => new(value);
}
