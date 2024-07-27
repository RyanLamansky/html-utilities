using static System.Text.Encoding;
using static HtmlUtilities.CodePointInfraCategory;

namespace HtmlUtilities;

public static class CodePointTests
{
    [Theory]
    [InlineData(0x00, ScalarValue | Ascii | Control | C0Control | C0ControlOrSpace)]
    [InlineData(0x09, ScalarValue | Ascii | Control | C0Control | C0ControlOrSpace | AsciiTabOrNewline | AsciiWhitespace)]
    [InlineData(0x0A, ScalarValue | Ascii | Control | C0Control | C0ControlOrSpace | AsciiTabOrNewline | AsciiWhitespace)]
    [InlineData(0x0C, ScalarValue | Ascii | Control | C0Control | C0ControlOrSpace | AsciiWhitespace)]
    [InlineData(0x0D, ScalarValue | Ascii | Control | C0Control | C0ControlOrSpace | AsciiTabOrNewline | AsciiWhitespace)]
    [InlineData(0x1F, ScalarValue | Ascii | Control | C0Control | C0ControlOrSpace)]
    [InlineData(' ', ScalarValue | Ascii | C0ControlOrSpace | AsciiWhitespace)]
    [InlineData('0', ScalarValue | Ascii | AsciiDigit | AsciiUpperHexDigit | AsciiLowerHexDigit | AsciiAlphanumeric)]
    [InlineData('9', ScalarValue | Ascii | AsciiDigit | AsciiUpperHexDigit | AsciiLowerHexDigit | AsciiAlphanumeric)]
    [InlineData('A', ScalarValue | Ascii | AsciiUpperHexDigit | AsciiHexDigit | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric)]
    [InlineData('F', ScalarValue | Ascii | AsciiUpperHexDigit | AsciiHexDigit | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric)]
    [InlineData('Z', ScalarValue | Ascii | AsciiUpperAlpha | AsciiAlpha | AsciiAlphanumeric)]
    [InlineData('a', ScalarValue | Ascii | AsciiLowerHexDigit | AsciiHexDigit | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric)]
    [InlineData('f', ScalarValue | Ascii | AsciiLowerHexDigit | AsciiHexDigit | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric)]
    [InlineData('z', ScalarValue | Ascii | AsciiLowerAlpha | AsciiAlpha | AsciiAlphanumeric)]
    [InlineData(0x7F, ScalarValue | Ascii | Control)]
    [InlineData(0x9F, ScalarValue | Control)]
    [InlineData('⌨', ScalarValue)]
    [InlineData(0xD800, Surrogate)]
    [InlineData(0xDFFF, Surrogate)]
    [InlineData(0xFFFE, ScalarValue | NonCharacter)]
    [InlineData(0x110000, None)]
    [InlineData(-1, None)]
    public static void CodePointsHaveCorrectInfraCategories(int codePoint, CodePointInfraCategory categories)
    {
        Assert.Equal(categories, new CodePoint(codePoint).InfraCategories);
    }

    public static readonly object?[][] Utf8ValidTestCases = [
        [null, Array.Empty<CodePoint>()],
        ["$", new CodePoint[] { 0x0024 }],
        ["£", new CodePoint[] { 0x00A3 }],
        ["ह", new CodePoint[] { 0x0939 }],
        ["€", new CodePoint[] { 0x20AC }],
        ["한", new CodePoint[] { 0xd55c }],
        ["𐍈", new CodePoint[] { 0x10348 }],
    ];

    [Theory]
    [MemberData(nameof(Utf8ValidTestCases))]
    public static void DecodeUtf8FromEnumerableBytes(string? value, CodePoint[] expected)
    {
        Assert.Equal(expected, CodePoint.DecodeUtf8(value is null ? null : UTF8.GetBytes(value)).ToArray());
    }

    [Theory]
    [MemberData(nameof(Utf8ValidTestCases))]
    public static void EncodeUtf8FromEnumerableCodePoints(string? expected, CodePoint[] value)
    {
        Assert.Equal(expected is null ? "" : expected, UTF8.GetString(CodePoint.EncodeUtf8(value).ToArray()));
    }

    public static readonly object?[][] Utf16TestCases = [
        [null, Array.Empty<CodePoint>()],
        ["$", new CodePoint[] { 0x0024 }],
        ["€", new CodePoint[] { 0x20AC }],
        ["𐐷", new CodePoint[] { 0x10437 }],
        ["𤭢", new CodePoint[] { 0x24B62 }],
    ];

    public static readonly object?[][] Utf16TestCasesWithInvalidCodePoints = [
        ["", new CodePoint[] { 0x110000 }],
        ["$", new CodePoint[] { 0x0024, 0x110000 }],
        ["$$", new CodePoint[] { 0x0024, 0x110000, 0x0024 }],
        ["$", new CodePoint[] { 0x110000, 0x0024 }],
    ];

    [Theory]
    [MemberData(nameof(Utf16TestCases))]
    public static void DecodeUtf16FromString(string? value, CodePoint[] expected)
    {
        Assert.Equal(expected, CodePoint.DecodeUtf16(value).ToArray());
    }

    [Theory]
    [MemberData(nameof(Utf16TestCases))]
    public static void DecodeUtf16FromEnumerableCharacters(string? value, CodePoint[] expected)
    {
        Assert.Equal(expected, CodePoint.DecodeUtf16((IEnumerable<char>?)value).ToArray());
    }

    [Theory]
    [MemberData(nameof(Utf16TestCases))]
    [MemberData(nameof(Utf16TestCasesWithInvalidCodePoints))]
    public static void EncodeUtf16FromEnumerableCodePoints(string? expected, CodePoint[] value)
    {
        Assert.Equal(expected is null ? "" : expected, CodePoint.EncodeUtf16(value).ToArray());
    }

    [Theory]
    [InlineData("$")]
    [InlineData("£")]
    [InlineData("ह")]
    [InlineData("€")]
    [InlineData("한")]
    [InlineData("𐍈")]
    public static void Utf8ByteCount(string value)
    {
        Assert.Equal(UTF8.GetByteCount(value), CodePoint.DecodeUtf16(value).Sum(cp => cp.Utf8ByteCount));
    }

    [Fact]
    public static void Utf8ByteCountIsZeroForInvalidCodePoints()
    {
        Assert.Equal(0, new CodePoint(0x110000).Utf8ByteCount);
    }

    [Theory]
    [InlineData("$")]
    [InlineData("£")]
    [InlineData("ह")]
    [InlineData("€")]
    [InlineData("한")]
    [InlineData("𐍈")]
    public static void Utf16ByteCount(string value)
    {
        Assert.Equal(Unicode.GetByteCount(value), CodePoint.DecodeUtf16(value).Sum(cp => cp.Utf16ByteCount));
    }

    [Fact]
    public static void Utf16ByteCountIsZeroForInvalidCodePoints()
    {
        Assert.Equal(0, new CodePoint(0x110000).Utf16ByteCount);
    }

    [Fact]
    public static void CompareTo()
    {
        const int test = 5;
        Assert.Equal(test.CompareTo(test), new CodePoint(test).CompareTo(test));
        Assert.Equal(test.CompareTo(test + 1), new CodePoint(test).CompareTo(test + 1));
        Assert.Equal(test.CompareTo(test - 1), new CodePoint(test).CompareTo(test - 1));
    }

    [Fact]
    public static void CompareToObject()
    {
        const int test = 5;
        Assert.Equal(test.CompareTo(test), ((IComparable)new CodePoint(test)).CompareTo(new CodePoint(test)));
        Assert.Equal(test.CompareTo(test + 1), ((IComparable)new CodePoint(test)).CompareTo(new CodePoint(test + 1)));
        Assert.Equal(test.CompareTo(test - 1), ((IComparable)new CodePoint(test)).CompareTo(new CodePoint(test - 1)));
    }

    [Fact]
    public static void CompareToObjectWrongType()
    {
        var x = Assert.Throws<ArgumentException>(() => ((IComparable)new CodePoint(5)).CompareTo(5));
        Assert.Equal("obj", x.ParamName);
    }

    [Fact]
    public static void EqualsObject()
    {
        Assert.False(new CodePoint('$').Equals(null));
        Assert.False(new CodePoint('$').Equals("$"));
        Assert.True(new CodePoint('$').Equals((object)(CodePoint)'$'));
    }

    [Fact]
    public static void HashCodeIsUniqueForEveryCodePoint()
    {
        const int maxCodePoint = 0x10FFFF, totalCodePoints = maxCodePoint + 1;
        Assert.Equal(totalCodePoints, Enumerable.Range(0, totalCodePoints).Select(cp => new CodePoint(cp).GetHashCode()).Distinct().Count());
    }

    [Theory]
    [InlineData("$", 0x0024)]
    [InlineData("€", 0x20AC)]
    [InlineData("𐐷", 0x10437)]
    [InlineData("𤭢", 0x24B62)]
    [InlineData("", 0x110000)]
    public static void ConvertToString(string expected, int codePoint)
    {
        Assert.Equal(expected, ((CodePoint)codePoint).ToString());
    }

    [Theory]
    [InlineData(0x0024)]
    [InlineData(0x20AC)]
    [InlineData(0x10437)]
    [InlineData(0x24B62)]
    public static void ConvertToStringWithFormatNullMatchesToString(int codePoint)
    {
        var cp = new CodePoint(codePoint);
        Assert.Equal(cp.ToString(), cp.ToString(null, null));
    }

    [Theory]
    [InlineData(0x0024)]
    [InlineData(0x20AC)]
    [InlineData(0x10437)]
    [InlineData(0x24B62)]
    public static void ConvertToStringWithFormatNullAndDefaultedFormatProviderMatchesToString(int codePoint)
    {
        var cp = new CodePoint(codePoint);
        Assert.Equal(cp.ToString(), cp.ToString(null));
    }

    [Theory]
    [InlineData(0x0024)]
    [InlineData(0x20AC)]
    [InlineData(0x10437)]
    [InlineData(0x24B62)]
    public static void ConvertToStringWithFormatBlankMatchesToString(int codePoint)
    {
        var cp = new CodePoint(codePoint);
        Assert.Equal(cp.ToString(), cp.ToString("", null));
    }

    [Theory]
    [InlineData(0x0024)]
    [InlineData(0x20AC)]
    [InlineData(0x10437)]
    [InlineData(0x24B62)]
    public static void ConvertToStringWithFormatX2MatchesUInt32ToStringX2(int codePoint)
    {
        var cp = new CodePoint(codePoint);
        Assert.Equal(codePoint.ToString("X2"), cp.ToString("X2", null));
    }

    [Theory]
    [InlineData("$", 0x0024)]
    [InlineData("€", 0x20AC)]
    [InlineData("𐐷", 0x10437)]
    [InlineData("𤭢", 0x24B62)]
    [InlineData("", 0x110000)]
    public static void TrySpanFormattable(string expected, int codePoint)
    {
        Span<char> destination = stackalloc char[2];
        Assert.True(new CodePoint(codePoint).TryFormat(destination, out var charsWritten));
        Assert.Equal(expected.Length, charsWritten);
        Assert.Equal(expected, new string(destination[..charsWritten]));
    }

    [Fact]
    public static void TrySpanFormattableTooShortIsFalse()
    {
        Assert.False(new CodePoint('T').TryFormat([], out var charsWritten));
        Assert.False(new CodePoint(0x24B62).TryFormat([], out charsWritten));

        Span<char> destination = stackalloc char[1];
        Assert.False(new CodePoint(0x24B62).TryFormat(destination, out charsWritten));
    }

    [Theory]
    [InlineData(0x0024)]
    [InlineData(0x20AC)]
    [InlineData(0x10437)]
    [InlineData(0x24B62)]
    public static void TrySpanFormattableWithFormatX2MatchesUInt32ToStringX22(int codePoint)
    {
        Span<char> destination = stackalloc char[10];

        Assert.True(codePoint.TryFormat(destination, out var charsWritten, "X2"));
        var expected = new string(destination[..charsWritten]);

        var cp = new CodePoint(codePoint);
        Assert.True(cp.TryFormat(destination, out charsWritten, "X2", null));
        var actual = new string(destination[..charsWritten]);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public static void OperatorEquals()
    {
        Assert.True(new CodePoint('$') == new CodePoint('$'));
        Assert.False(new CodePoint('$') == new CodePoint('€'));
    }

    [Fact]
    public static void OperatorNotEquals()
    {
        Assert.False(new CodePoint('$') != new CodePoint('$'));
        Assert.True(new CodePoint('$') != new CodePoint('€'));
    }

    [Fact]
    public static void OperatorLessThan()
    {
        Assert.False(new CodePoint('$') < new CodePoint('$'));
        Assert.False(new CodePoint('€') < new CodePoint('$'));
        Assert.True(new CodePoint('$') < new CodePoint('€'));
    }

    [Fact]
    public static void OperatorGreaterThan()
    {
        Assert.False(new CodePoint('$') > new CodePoint('$'));
        Assert.False(new CodePoint('$') > new CodePoint('€'));
        Assert.True(new CodePoint('€') > new CodePoint('$'));
    }

    [Fact]
    public static void OperatorLessThanOrEqualTo()
    {
        Assert.True(new CodePoint('$') <= new CodePoint('$'));
        Assert.False(new CodePoint('€') <= new CodePoint('$'));
        Assert.True(new CodePoint('$') <= new CodePoint('€'));
    }

    [Fact]
    public static void OperatorGreaterThanOrEqualTo()
    {
        Assert.True(new CodePoint('$') >= new CodePoint('$'));
        Assert.False(new CodePoint('$') >= new CodePoint('€'));
        Assert.True(new CodePoint('€') >= new CodePoint('$'));
    }

    [Fact]
    public static void ConversionOperators()
    {
        CodePoint cp;

        // Implicit conversion to CodePoint
        cp = (byte)'$';
        Assert.Equal(new CodePoint('$'), cp);

        cp = '€'; //char
        Assert.Equal(new CodePoint('€'), cp);

        cp = (int)'€';
        Assert.Equal(new CodePoint('€'), cp);

        cp = (uint)'€';
        Assert.Equal(new CodePoint('€'), cp);

        // Implicit conversion from CodePoint.
        cp = '€';
        Assert.Equal<int>('€', cp);

        cp = '€';
        Assert.Equal<uint>('€', cp);

        // Explicit conversion from CodePoint.
        cp = '€';
        Assert.Equal<char>('€', (char)cp);

        Assert.Throws<OverflowException>(() => (char)new CodePoint(0x24B62));
    }

    [Theory]
    [MemberData(nameof(Utf16TestCases))]
    public static void DecodeUtf16FromStackString(string? value, CodePoint[] expected)
    {
        var results = new List<CodePoint>();
        foreach (var codePoint in CodePoint.GetEnumerable(value))
            results.Add(codePoint);

        Assert.Equal(expected, results);
    }

    [Theory]
    [MemberData(nameof(Utf16TestCases))]
    public static void DecodeUtf16FromStackReadOnlySpan(string? value, CodePoint[] expected)
    {
        var results = new List<CodePoint>();
        foreach (var codePoint in CodePoint.GetEnumerable(value.AsSpan()))
            results.Add(codePoint);

        Assert.Equal(expected, results);
    }

    [Theory]
    [InlineData("Test", 4)]
    [InlineData("日本語", 9)]
    public static void Utf8BytesNeeded(string value, int utf8ByteLength)
    {
        Assert.Equal(utf8ByteLength, CodePoint.Utf8BytesNeeded(value));
    }

    [Theory]
    [InlineData("Test", new byte[] { 84, 101, 115, 116 })]
    [InlineData("日本語", new byte[] { 230, 151, 165, 230, 156, 172, 232, 170, 158 })]
    public static void SwitchUtf16To8(string value, byte[] expected)
    {
        var destination = new byte[CodePoint.Utf8BytesNeeded(value)];

        CodePoint.SwitchUtf(value, destination);

        Assert.Equal(expected, destination);
    }
}