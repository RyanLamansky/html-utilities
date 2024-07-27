using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static HtmlUtilities.CodePointInfraCategory;

namespace HtmlUtilities;

/// <summary>
/// Represents a single Unicode code point as described by https://infra.spec.whatwg.org/#code-points.
/// Also provided are several mechanisms to convert to and from <see cref="CodePoint"/> values.
/// </summary>
public readonly struct CodePoint : IEquatable<CodePoint>, IComparable, IComparable<CodePoint>, ISpanFormattable, IFormattable
{
    /// <summary>
    /// Gets the raw Unicode code point value.
    /// Valid code points are in the range of 0 through 0x10FFFF (1114111 in decimal), but <see cref="CodePoint"/> accepts the full range of <see cref="uint"/>.
    /// </summary>
    public uint Value { get; }

    /// <summary>
    /// Creates a new <see cref="CodePoint"/> with the provided raw Unicode value.
    /// </summary>
    /// <param name="value">The raw Unicode code point value.</param>
    public CodePoint(int value) : this((uint)value)
    {
    }

    /// <summary>
    /// Creates a new <see cref="CodePoint"/> with the provided raw Unicode value.
    /// </summary>
    /// <param name="value">The raw Unicode code point value.</param>
    public CodePoint(uint value)
    {
        this.Value = value;
    }

    // This pre-calculated lookup table provides O(1) lookup time for ASCII characters.
    // https://github.com/dotnet/runtime/issues/60948 (via https://github.com/dotnet/roslyn/pull/61414) can potentially make this faster.
    // It would also save 512 bytes + overhead of this statically allocated array.
    // The current approach was the fastest known option at the time it was written.
    private static readonly CodePointInfraCategory[] AsciiInfraCategories = new[]
    {
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
    };

    /// <summary>
    /// Gets the categories of a <see cref="CodePoint"/> as defined by <a href="https://infra.spec.whatwg.org/#code-points">the "infra" standard</a>.
    /// Code points outside the range of 0 through 0x10FFFF (1114111 in decimal) always return <see cref="None"/>.
    /// </summary>
    public CodePointInfraCategory InfraCategories
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            // Considering that this is an HTML-oriented project, ASCII will be very common so we have a fast path for that.
            if (Value < AsciiInfraCategories.Length)
                return AsciiInfraCategories[Value];

            return NonAsciiInfraCategory(Value);
        }
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

    /// <summary>
    /// Gets the number of bytes required to encode this code point with UTF-8.
    /// </summary>
    public int Utf8ByteCount => Value switch
    {
        <= 0x7F => 1,
        <= 0x7FF => 2,
        <= 0xFFFF => 3,
        <= 0x10FFFF => 4,
        _ => 0
    };

    /// <summary>
    /// Gets the number of bytes required to encode this code point with UTF-16.
    /// </summary>
    public int Utf16ByteCount => Value switch
    {
        <= 0xD7FF or >= 0xE000 and <= 0xFFFF => 2,
        <= 0x10FFFF => 4,
        _ => 0
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteUtf8To(Span<byte> destination, ref int location)
    {
        if (Value <= 0x7F)
        {
            destination[location++] = (byte)Value;
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

        WriteUtf8ToNonAscii(Value, destination, ref location);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void WriteUtf8To(ref ArrayBuilder<byte> writer)
    {
        var value = this.Value;

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

    /// <inheritdoc />
    public int CompareTo(CodePoint other) => this.Value.CompareTo(other.Value);

    int IComparable.CompareTo(object? obj)
    {
        if (obj is not CodePoint codePoint)
            throw new ArgumentException("obj is not a CodePoint.", nameof(obj));

        return CompareTo(codePoint);
    }

    /// <inheritdoc />
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is CodePoint cp && Equals(cp);

    /// <summary>
    /// Returns true if the current <see cref="Value"/> matches the one provided.
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
            return ((char)value).ToString();

        if (value > 0x10FFFF)
            return string.Empty;

        Span<char> chars = stackalloc char[2];

        value -= 0x10000;
        chars[0] = (char)(value / 0x400 + 0xD800);
        chars[1] = (char)(value % 0x400 + 0xDC00);

        return new string(chars);
    }

    /// <inheritdoc />
    public string ToString(string? format, IFormatProvider? formatProvider = null) =>
        string.IsNullOrEmpty(format) ? ToString() : Value.ToString(format, formatProvider);

    /// <inheritdoc />
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        if (!format.IsEmpty)
            return Value.TryFormat(destination, out charsWritten, format, provider);

        var value = this.Value;

        if (value <= 0xD7FF || (value >= 0xE000 && value <= 0xFFFF))
        {
            if (destination.IsEmpty)
            {
                charsWritten = 0;
                return false;
            }

            destination[0] = (char)value;

            charsWritten = 1;
            return true;
        }

        if (value > 0x10FFFF)
        {
            charsWritten = 0;
            return true;
        }

        if (destination.Length < 2)
        {
            charsWritten = 0;
            return false;
        }

        value -= 0x10000;
        destination[0] = (char)(value / 0x400 + 0xD800);
        destination[1] = (char)(value % 0x400 + 0xDC00);

        charsWritten = 2;
        return true;
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
    public static implicit operator CodePoint(int value) => new((uint)value);

    /// <summary>
    /// Creates a new <see cref="CodePoint"/> value from the provided value.
    /// </summary>
    /// <param name="value">The raw Unicode code point value.</param>
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
    public static IEnumerable<CodePoint> DecodeUtf8(IEnumerable<byte>? source)
    {
        if (source is null)
            yield break;

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
    public static IEnumerable<CodePoint> DecodeUtf16(IEnumerable<char>? source)
    {
        if (source is null)
            yield break;

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
    public static IEnumerable<CodePoint> DecodeUtf16(string? source)
    {
        if (source is null)
            yield break;

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
    /// Encodes a sequence of code points into a UTF-16 sequence.
    /// </summary>
    /// <param name="source">The sequence of code points.</param>
    /// <returns>The UTF-16 sequence.</returns>
    public static IEnumerable<char> EncodeUtf16(IEnumerable<CodePoint>? source)
    {
        if (source is null)
            yield break;

        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var value = enumerator.Current.Value;

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
    public static IEnumerable<byte> EncodeUtf8(IEnumerable<CodePoint>? source)
    {
        if (source is null)
            yield break;

        using var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var current = enumerator.Current.Value;

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
    /// Gets an enumerable for <see cref="CodePoint"/>s from a <see cref="ReadOnlySpan{T}"/> of type <see cref="byte"/> without allocating heap memory.
    /// </summary>
    public static Utf8DecoderEnumerable GetEnumerable(ReadOnlySpan<byte> source) => new(source);

    /// <summary>
    /// Gets an enumerable for <see cref="CodePoint"/>s from a <see cref="ReadOnlySpan{T}"/> of type <see cref="char"/> without allocating heap memory.
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
            checked { bytesNeeded += codePoint.Utf8ByteCount; }

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
