using System.Diagnostics.CodeAnalysis;

namespace HtmlUtilities;

/// <summary>
/// Converts data to or from Unicode code points.
/// </summary>
public readonly struct CodePoint : IEquatable<CodePoint>, IComparable<CodePoint>
{
    /// <summary>
    /// Gets the raw Unicode code point value.
    /// </summary>
    public readonly uint Value { get; init; }

    /// <summary>
    /// Creates a new <see cref="CodePoint"/> with the provided raw Unicode value.
    /// </summary>
    /// <param name="value">The raw Unicode code point value.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> must be in the ange of 0 through 1112064 (0x10FFFF).</exception>
    public CodePoint(int value) : this((uint)value)
    {
    }

    /// <summary>
    /// Creates a new <see cref="CodePoint"/> with the provided raw Unicode value.
    /// </summary>
    /// <param name="value">The raw Unicode code point value.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> must be in the range of 0 through 1112064 (0x10FFFF).</exception>
    public CodePoint(uint value)
    {
        if (value > 0x10FFFF)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be in the range of 0 through 1112064 (0x10FFFF).");

        this.Value = value;
    }

    /// <summary>
    /// Gets the number of bytes required to encode this code point with UTF-8.
    /// </summary>
    public int Utf8ByteCount
    {
        get
        {
            var value = this.Value;

            if (value <= 0x7F)
                return 1;
            if (value <= 0x7FF)
                return 2;
            if (value <= 0xFFFF)
                return 3;
            if (value <= 0x10FFFF)
                return 4;

            return 0; // This should only be possible if the value was altered via direct memory edit.
        }
    }
    /// <summary>
    /// Gets the number of bytes required to encode this code point with UTF-16.
    /// </summary>
    public int Utf16ByteCount
    {
        get
        {
            var value = this.Value;

            if (value <= 0xD7FF || (value >= 0xE000 && value <= 0xFFFF))
            {
                return 2;
            }

            return 4;
        }
    }

    /// <inheritdoc />
    public int CompareTo(CodePoint other) => this.Value.CompareTo(other.Value);

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is CodePoint cp && Equals(cp);

    /// <summary>
    /// Returns true of the current <see cref="Value"/> matches the one provided.
    /// </summary>
    /// <param name="other">Another value.</param>
    /// <returns>True if they match, otherwise false.</returns>
    public bool Equals(CodePoint other) => other.Value == this.Value;

    /// <summary>
    /// Returns <see cref="Value"/> cast to an <see cref="int"/>.
    /// </summary>
    /// <returns><see cref="Value"/> cast to an <see cref="int"/>.</returns>
    public override int GetHashCode() => (int)this.Value;

    /// <summary>
    /// Converts <see cref="Value"/> into a string.
    /// </summary>
    /// <returns>The string representation of <see cref="Value"/>.</returns>
    public override string ToString()
    {
        var value = this.Value;

        if (value <= 0xD7FF || (value >= 0xE000 && value <= 0xFFFF))
        {
            return ((char)value).ToString();
        }

        Span<char> chars = stackalloc char[2];

        value -= 0x10000;
        chars[0] = (char)(value / 0x400 + 0xD800);
        chars[1] = (char)(value % 0x400 + 0xDC00);

        return new string(chars);
    }

    /// <summary>
    /// Compares two <see cref="CodePoint"/> values for equality.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="right">The right side of the comparison.</param>
    /// <returns>True if the values match, otherwise false.</returns>
    public static bool operator ==(CodePoint left, CodePoint right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="CodePoint"/> values for inequality.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="right">The right side of the comparison.</param>
    /// <returns>True if the values do not match, otherwise false.</returns>
    public static bool operator !=(CodePoint left, CodePoint right) => !(left == right);

    /// <summary>
    /// Compares determines whether the left side <see cref="CodePoint"/> is less than the right side.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="right">The right side of the comparison.</param>
    /// <returns>True if <paramref name="left"/> is less than <paramref name="right"/>, otherwise false.</returns>
    public static bool operator <(CodePoint left, CodePoint right) => left.Value < right.Value;

    /// <summary>
    /// Compares determines whether the left side <see cref="CodePoint"/> is greater than the right side.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="right">The right side of the comparison.</param>
    /// <returns>True if <paramref name="left"/> is greater than <paramref name="right"/>, otherwise false.</returns>
    public static bool operator >(CodePoint left, CodePoint right) => left.Value > right.Value;

    /// <summary>
    /// Compares determines whether the left side <see cref="CodePoint"/> is less than or equal to the right side.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="right">The right side of the comparison.</param>
    /// <returns>True if <paramref name="left"/> is less than or equal to <paramref name="right"/>, otherwise false.</returns>
    public static bool operator <=(CodePoint left, CodePoint right) => left.Value <= right.Value;

    /// <summary>
    /// Compares determines whether the left side <see cref="CodePoint"/> is greater than or equal to than the right side.
    /// </summary>
    /// <param name="left">The left side of the comparison.</param>
    /// <param name="right">The right side of the comparison.</param>
    /// <returns>True if <paramref name="left"/> is greater than or equal to <paramref name="right"/>, otherwise false.</returns>
    public static bool operator >=(CodePoint left, CodePoint right) => left.Value >= right.Value;

    /// <summary>
    /// Creates a new <see cref="CodePoint"/> value from the provided value.
    /// </summary>
    /// <param name="value">The raw Unicode code point value.</param>
    public static implicit operator CodePoint(byte value) => new(value);

    /// <summary>
    /// Creates a new <see cref="CodePoint"/> value from the provided value.
    /// </summary>
    /// <param name="value">The raw Unicode code point value.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> must be in the ange of 0 through 1112064 (0x10FFFF).</exception>
    public static implicit operator CodePoint(int value) => new(value);

    /// <summary>
    /// Creates a new <see cref="CodePoint"/> value from the provided value.
    /// </summary>
    /// <param name="value">The raw Unicode code point value.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> must be in the ange of 0 through 1112064 (0x10FFFF).</exception>
    public static implicit operator CodePoint(uint value) => new(value);

    /// <summary>
    /// Creates a new <see cref="CodePoint"/> value from the provided value.
    /// </summary>
    /// <param name="value">The raw Unicode code point value.</param>
    public static implicit operator CodePoint(char value) => new(value);

    /// <summary>
    /// Losslessly converts the <see cref="CodePoint"/> value into an <see cref="int"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator int(CodePoint value) => (int)value.Value;

    /// <summary>
    /// Losslessly converts the <see cref="CodePoint"/> value into an <see cref="uint"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator uint(CodePoint value) => value.Value;

    /// <summary>
    /// Converts the <see cref="CodePoint"/> value into an <see cref="char"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <exception cref="OverflowException">The <see cref="Value"/> of the provided <see cref="CodePoint"/> can't be losslessly converted to <see cref="char"/>.</exception>
    public static explicit operator char(CodePoint value) => checked((char)value.Value);

    /// <summary>
    /// Decodes a sequence of UTF-8 bytes into Unicode code points. Invalid bytes are skipped.
    /// </summary>
    /// <param name="source">The sequence of bytes.</param>
    /// <returns>The sequence of code points.</returns>
    public static IEnumerable<CodePoint> DecodeUtf8(IEnumerable<byte> source)
    {
        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;

            if (current <= 0x7f)
            {
                yield return current;
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

                yield return (b1 << 6) | current & 0b00111111;
            }
            else if ((current >> 4) == 0b1110)
            {
                b1 = current & 0b00001111;
                if (!Next(enumerator, ref current))
                    continue; // Invalid sequence.

                b2 = current & 0b00111111;
                if (!Next(enumerator, ref current))
                    continue; // Invalid sequence.

                yield return (b1 << 12) | (b2 << 6) | current & 0b00111111;
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

                yield return (b1 << 18) | (b2 << 12) | (b3 << 6) | current & 0b00111111;
            }
        }
    }

    /// <summary>
    /// Decodes a UTF-16 sequence into Unicode code points.
    /// </summary>
    /// <param name="source">The UTF-16 sequence.</param>
    /// <returns>The sequence of code points.</returns>
    public static IEnumerable<CodePoint> DecodeUtf16(IEnumerable<char> source)
    {
        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var high = (int)enumerator.Current;

            if (high <= 0xD7FF || (high >= 0xE000 && high <= 0xFFFF))
            {
                yield return high;
                continue;
            }

            if (!enumerator.MoveNext())
                continue;

            var low = (int)enumerator.Current;

            yield return (high - 0xD800) * 0x400 + (low - 0xDC00) + 0x10000;
        }
    }

    /// <summary>
    /// Decodes a <see cref="string"/> into Unicode code points.
    /// </summary>
    /// <param name="source">The string to decode.</param>
    /// <returns>The sequence of code points.</returns>
    /// <remarks>This method leverages <see cref="CharEnumerator"/> for better performance than <see cref="DecodeUtf16(IEnumerable{char})"/>.</remarks>
    public static IEnumerable<CodePoint> DecodeUtf16(string source)
    {
        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var high = (int)enumerator.Current;

            if (high <= 0xD7FF || (high >= 0xE000 && high <= 0xFFFF))
            {
                yield return high;
                continue;
            }

            if (!enumerator.MoveNext())
                continue;

            var low = (int)enumerator.Current;

            yield return (high - 0xD800) * 0x400 + (low - 0xDC00) + 0x10000;
        }
    }

    /// <summary>
    /// Encodes a sequence of code points into a UTF-16 sequece.
    /// </summary>
    /// <param name="source">The sequence of code points.</param>
    /// <returns>The UTF-16 sequence.</returns>
    public static IEnumerable<char> EncodeUtf16(IEnumerable<CodePoint> source)
    {
        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var value = enumerator.Current.Value;

            if (value <= 0xD7FF || (value >= 0xE000 && value <= 0xFFFF))
            {
                yield return (char)value;
                continue;
            }

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
    public static IEnumerable<byte> EncodeUtf8(IEnumerable<CodePoint> source)
    {
        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;

            if (current <= 0x7F)
                yield return (byte)current.Value;
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
}
