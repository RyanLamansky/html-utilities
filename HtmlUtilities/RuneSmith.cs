using System.Runtime.CompilerServices;
using static HtmlUtilities.CodePointInfraCategory;
using Rune = System.Text.Rune;

namespace HtmlUtilities;

/// <summary>
/// Adds features to the <see cref="Rune"/> class.
/// </summary>
public static class RuneSmith
{
    // This pre-calculated lookup table provides O(1) lookup time for ASCII characters.
    // https://github.com/dotnet/runtime/issues/60948 (via https://github.com/dotnet/roslyn/pull/61414) accelerates it.
    // It works by creating a ReadOnlySpan into a compile-time generated constant.
    private static ReadOnlySpan<CodePointInfraCategory> AsciiInfraCategories =>
    [
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | AsciiTabOrNewline | AsciiWhitespace | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | AsciiTabOrNewline | AsciiWhitespace | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | AsciiWhitespace | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | AsciiTabOrNewline | AsciiWhitespace | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | C0Control | C0ControlOrSpace | Control,
        ScalarValue | Ascii | AsciiWhitespace | C0ControlOrSpace,
        ScalarValue | Ascii,
        ScalarValue | Ascii,
        ScalarValue | Ascii,
        ScalarValue | Ascii,
        ScalarValue | Ascii,
        ScalarValue | Ascii,
        ScalarValue | Ascii,
        ScalarValue | Ascii,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiDigit | AsciiUpperHexDigit | AsciiLowerHexDigit | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiDigit | AsciiUpperHexDigit | AsciiLowerHexDigit | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiDigit | AsciiUpperHexDigit | AsciiLowerHexDigit | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiDigit | AsciiUpperHexDigit | AsciiLowerHexDigit | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiDigit | AsciiUpperHexDigit | AsciiLowerHexDigit | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiDigit | AsciiUpperHexDigit | AsciiLowerHexDigit | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiDigit | AsciiUpperHexDigit | AsciiLowerHexDigit | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiDigit | AsciiUpperHexDigit | AsciiLowerHexDigit | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiDigit | AsciiUpperHexDigit | AsciiLowerHexDigit | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiDigit | AsciiUpperHexDigit | AsciiLowerHexDigit | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperHexDigit | AsciiHexDigit | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperHexDigit | AsciiHexDigit | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperHexDigit | AsciiHexDigit | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperHexDigit | AsciiHexDigit | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperHexDigit | AsciiHexDigit | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperHexDigit | AsciiHexDigit | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerHexDigit | AsciiHexDigit | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerHexDigit | AsciiHexDigit | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerHexDigit | AsciiHexDigit | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerHexDigit | AsciiHexDigit | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerHexDigit | AsciiHexDigit | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerHexDigit | AsciiHexDigit | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric,
        ScalarValue | Ascii,
        ScalarValue | Ascii,
        ScalarValue | Ascii,
        ScalarValue | Ascii,
        ScalarValue | Ascii | Control,
    ];

    /// <summary>
    /// Gets the categories of a <see cref="Rune"/> as defined by <a href="https://infra.spec.whatwg.org/#code-points">the "infra" standard</a>.
    /// Code points outside the range of 0 through 0x10FFFF (1114111 in decimal) always return <see cref="None"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CodePointInfraCategory InfraCategories(this Rune rune)
    {
        var value = (uint)rune.Value;
        // Considering that this is an HTML-oriented project, ASCII will be very common so we have a fast path for that.
        if (value < AsciiInfraCategories.Length)
            return AsciiInfraCategories[(int)value];

        return NonAsciiInfraCategory(value);
    }

    private static CodePointInfraCategory NonAsciiInfraCategory(uint codePoint) => codePoint switch
    {
        <= 0x9F => ScalarValue | Control,
        >= 0xD800 and <= 0xDFFF => Surrogate,
        >= 0xFDD0 and <= 0xFDEF
        or 0xFFFE or 0xFFFF or 0x1FFFE or 0x1FFFF or 0x2FFFE or 0x2FFFF
        or 0x3FFFE or 0x3FFFF or 0x4FFFE or 0x4FFFF or 0x5FFFE or 0x5FFFF
        or 0x6FFFE or 0x6FFFF or 0x7FFFE or 0x7FFFF or 0x8FFFE or 0x8FFFF
        or 0x9FFFE or 0x9FFFF or 0xAFFFE or 0xAFFFF or 0xBFFFE or 0xBFFFF
        or 0xCFFFE or 0xCFFFF or 0xDFFFE or 0xDFFFF or 0xEFFFE or 0xEFFFF
        or 0xFFFFE or 0xFFFFF or 0x10FFFE or 0x10FFFF
        => ScalarValue | NonCharacter,
        > 0x10FFFF => None,
        _ => ScalarValue,
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteUtf8To(this Rune rune, Span<byte> destination, ref int location)
    {
        var value = (uint)rune.Value;
        if (value <= 0x7F)
        {
            destination[location++] = (byte)value;
            return;
        }

        static void WriteUtf8ToNonAscii(uint value, Span<byte> destination, ref int location)
        {
            if (value <= 0x7FF)
            {
                destination[location++] = (byte)(0b11000000 | (value >> 6));
                destination[location++] = (byte)(0b10000000 | value & 0b00111111);
                return;
            }

            if (value <= 0xFFFF)
            {
                destination[location++] = (byte)(0b11100000 | (value >> 12));
                destination[location++] = (byte)(0b10000000 | (value >> 6) & 0b00111111);
                destination[location++] = (byte)(0b10000000 | value & 0b00111111);
                return;
            }

            if (value <= 0x10FFFF)
            {
                destination[location++] = (byte)(0b11110000 | (value >> 18));
                destination[location++] = (byte)(0b10000000 | (value >> 12) & 0b00111111);
                destination[location++] = (byte)(0b10000000 | (value >> 6) & 0b00111111);
                destination[location++] = (byte)(0b10000000 | value & 0b00111111);
                return;
            }
        }

        WriteUtf8ToNonAscii(value, destination, ref location);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void WriteUtf8To(this Rune rune, ref ArrayBuilder<byte> writer)
    {
        var value = (uint)rune.Value;

        if (value <= 0x7F)
        {
            writer.Write((byte)value);
            return;
        }

        static void WriteUtf8ToNonAscii(ref ArrayBuilder<byte> writer, uint value)
        {
            if (value <= 0x7FF)
            {
                var destination = writer.GetSpan(2);

                destination[0] = (byte)(0b11000000 | (value >> 6));
                destination[1] = (byte)(0b10000000 | value & 0b00111111);
                return;
            }

            if (value <= 0xFFFF)
            {
                var destination = writer.GetSpan(3);

                destination[0] = (byte)(0b11100000 | (value >> 12));
                destination[1] = (byte)(0b10000000 | (value >> 6) & 0b00111111);
                destination[2] = (byte)(0b10000000 | value & 0b00111111);
                return;
            }

            if (value <= 0x10FFFF)
            {
                var destination = writer.GetSpan(4);

                destination[0] = (byte)(0b11110000 | (value >> 18));
                destination[1] = (byte)(0b10000000 | (value >> 12) & 0b00111111);
                destination[2] = (byte)(0b10000000 | (value >> 6) & 0b00111111);
                destination[3] = (byte)(0b10000000 | value & 0b00111111);
                return;
            }
        }

        WriteUtf8ToNonAscii(ref writer, value);
    }

    /// <summary>
    /// Decodes a sequence of UTF-8 bytes into Unicode code points. Invalid bytes are skipped.
    /// </summary>
    /// <param name="source">The sequence of bytes.</param>
    /// <returns>The sequence of code points.</returns>
    public static IEnumerable<Rune> DecodeUtf8(IEnumerable<byte>? source)
    {
        if (source is null)
            yield break;

        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;

            if (current <= 0x7f)
            {
                yield return new(current);
                continue;
            }

            static bool Next(IEnumerator<byte> enumerator, ref byte current)
            {
                if (!enumerator.MoveNext())
                    return false; // Invalid sequence.
                if (((current = enumerator.Current) >> 6) != 0b10)
                    return false;

                return true;
            }

            int b1, b2, b3;
            if ((current >> 5) == 0b110)
            {
                b1 = current & 0b00011111;
                if (!Next(enumerator, ref current))
                    continue; // Invalid sequence.

                yield return new((b1 << 6) | current & 0b00111111);
            }
            else if ((current >> 4) == 0b1110)
            {
                b1 = current & 0b00001111;
                if (!Next(enumerator, ref current))
                    continue; // Invalid sequence.

                b2 = current & 0b00111111;
                if (!Next(enumerator, ref current))
                    continue; // Invalid sequence.

                yield return new((b1 << 12) | (b2 << 6) | current & 0b00111111);
            }
            else if ((current >> 3) == 0b11110)
            {
                b1 = current & 0b00001111;
                if (!Next(enumerator, ref current))
                    continue; // Invalid sequence.

                b2 = current & 0b00111111;
                if (!Next(enumerator, ref current))
                    continue; // Invalid sequence.

                b3 = current & 0b00111111;
                if (!Next(enumerator, ref current))
                    continue; // Invalid sequence.

                yield return new((b1 << 18) | (b2 << 12) | (b3 << 6) | current & 0b00111111);
            }
        }
    }

    /// <summary>
    /// Decodes a UTF-16 sequence into Unicode code points.
    /// </summary>
    /// <param name="source">The UTF-16 sequence.</param>
    /// <returns>The sequence of code points.</returns>
    public static IEnumerable<Rune> DecodeUtf16(IEnumerable<char>? source)
    {
        if (source is null)
            yield break;

        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var high = (int)enumerator.Current;

            if (high <= 0xD7FF || (high >= 0xE000 && high <= 0xFFFF))
            {
                yield return new(high);
                continue;
            }

            if (!enumerator.MoveNext())
                continue;

            var low = (int)enumerator.Current;

            yield return new((high - 0xD800) * 0x400 + (low - 0xDC00) + 0x10000);
        }
    }

    /// <summary>
    /// Decodes a <see cref="string"/> into Unicode code points.
    /// </summary>
    /// <param name="source">The string to decode.</param>
    /// <returns>The sequence of code points.</returns>
    /// <remarks>This method leverages <see cref="CharEnumerator"/> for better performance than <see cref="DecodeUtf16(IEnumerable{char})"/>.</remarks>
    public static IEnumerable<Rune> DecodeUtf16(string? source)
    {
        if (source is null)
            yield break;

        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var high = (int)enumerator.Current;

            if (high <= 0xD7FF || (high >= 0xE000 && high <= 0xFFFF))
            {
                yield return new(high);
                continue;
            }

            if (!enumerator.MoveNext())
                continue;

            var low = (int)enumerator.Current;

            yield return new((high - 0xD800) * 0x400 + (low - 0xDC00) + 0x10000);
        }
    }

    /// <summary>
    /// Encodes a sequence of code points into a UTF-16 sequence.
    /// </summary>
    /// <param name="source">The sequence of code points.</param>
    /// <returns>The UTF-16 sequence.</returns>
    public static IEnumerable<char> EncodeUtf16(IEnumerable<Rune>? source)
    {
        if (source is null)
            yield break;

        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var value = (uint)enumerator.Current.Value;

            if (value <= 0xD7FF || (value >= 0xE000 && value <= 0xFFFF))
            {
                yield return (char)value;
                continue;
            }

            if (value > 0x10FFFF)
                continue;

            value -= 0x10000;
            yield return (char)(value / 0x400 + 0xD800);
            yield return (char)(value % 0x400 + 0xDC00);
        }
    }

    /// <summary>
    /// Encodes a sequence of Unicode code points into UTF-8 bytes. Invalid code points are skipped.
    /// </summary>
    /// <param name="source">The sequence of code points.</param>
    /// <returns>The sequence of bytes.</returns>
    public static IEnumerable<byte> EncodeUtf8(IEnumerable<Rune>? source)
    {
        if (source is null)
            yield break;

        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var current = (uint)enumerator.Current.Value;

            if (current <= 0x7F)
                yield return (byte)current;
            else if (current <= 0x7FF)
            {
                yield return (byte)(0b11000000 | (current >> 6));
                yield return (byte)(0b10000000 | current & 0b00111111);
            }
            else if (current <= 0xFFFF)
            {
                yield return (byte)(0b11100000 | (current >> 12));
                yield return (byte)(0b10000000 | (current >> 6) & 0b00111111);
                yield return (byte)(0b10000000 | current & 0b00111111);
            }
            else if (current <= 0x10FFFF)
            {
                yield return (byte)(0b11110000 | (current >> 18));
                yield return (byte)(0b10000000 | (current >> 12) & 0b00111111);
                yield return (byte)(0b10000000 | (current >> 6) & 0b00111111);
                yield return (byte)(0b10000000 | current & 0b00111111);
            }
        }
    }

    /// <summary>
    /// Gets an enumerable for <see cref="Rune"/>s from a <see cref="ReadOnlySpan{T}"/> of type <see cref="byte"/> without allocating heap memory.
    /// </summary>
    public static Utf8DecoderEnumerable GetEnumerable(ReadOnlySpan<byte> source) => new(source);

    /// <summary>
    /// Gets an enumerable for <see cref="Rune"/>s from a <see cref="ReadOnlySpan{T}"/> of type <see cref="char"/> without allocating heap memory.
    /// </summary>
    public static Utf16DecoderEnumerable GetEnumerable(ReadOnlySpan<char> source) => new(source);

    /// <summary>
    /// Determines the amount of UTF-8 bytes needed for a UTF-16 source without allocating heap memory.
    /// </summary>
    /// <param name="source">A sequence of UTF-16 characters.</param>
    /// <returns>The number of bytes required.</returns>
    /// <exception cref="OverflowException">More than <see cref="int.MaxValue"/> bytes are required.</exception>
    public static int Utf8BytesNeeded(ReadOnlySpan<char> source)
    {
        var enumerable = new Utf16DecoderEnumerable(source);
        var bytesNeeded = 0;
        foreach (var codePoint in enumerable)
            checked { bytesNeeded += codePoint.Utf8SequenceLength; }

        return bytesNeeded;
    }

    /// <summary>
    /// Converts UTF-16 to UTF-8 without allocating heap memory.
    /// </summary>
    /// <param name="source">A sequence of UTF-16 characters.</param>
    /// <param name="destination">The destination for UTF-8 bytes.</param>
    /// <returns>The number of bytes required.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="destination"/> is not large enough to contain the converted input.</exception>
    public static int SwitchUtf(ReadOnlySpan<char> source, Span<byte> destination)
    {
        var enumerable = new Utf16DecoderEnumerable(source);
        var bytesWritten = 0;

        foreach (var codePoint in enumerable)
        {
            try
            {
                codePoint.WriteUtf8To(destination, ref bytesWritten);
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentOutOfRangeException(nameof(destination), "destination is not large enough to contain the converted input.");
            }
        }

        return bytesWritten;
    }
}
