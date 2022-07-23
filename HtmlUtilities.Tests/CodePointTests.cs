using static System.Text.Encoding;

namespace HtmlUtilities;

public static class CodePointTests
{
    [Theory]
    [InlineData(0x00, CodePointInfraCategory.ScalarValue | CodePointInfraCategory.Ascii
        | CodePointInfraCategory.Control
        | CodePointInfraCategory.C0Control
        | CodePointInfraCategory.C0ControlOrSpace)]
    [InlineData(0x09, CodePointInfraCategory.ScalarValue | CodePointInfraCategory.Ascii
        | CodePointInfraCategory.Control
        | CodePointInfraCategory.C0Control
        | CodePointInfraCategory.C0ControlOrSpace
        | CodePointInfraCategory.AsciiTabOrNewline
        | CodePointInfraCategory.AsciiWhitespace)]
    [InlineData(0x0A, CodePointInfraCategory.ScalarValue | CodePointInfraCategory.Ascii
        | CodePointInfraCategory.Control
        | CodePointInfraCategory.C0Control
        | CodePointInfraCategory.C0ControlOrSpace
        | CodePointInfraCategory.AsciiTabOrNewline
        | CodePointInfraCategory.AsciiWhitespace)]
    [InlineData(0x0C, CodePointInfraCategory.ScalarValue | CodePointInfraCategory.Ascii
        | CodePointInfraCategory.Control
        | CodePointInfraCategory.C0Control
        | CodePointInfraCategory.C0ControlOrSpace
        | CodePointInfraCategory.AsciiWhitespace)]
    [InlineData(0x0D, CodePointInfraCategory.ScalarValue | CodePointInfraCategory.Ascii
        | CodePointInfraCategory.Control
        | CodePointInfraCategory.C0Control
        | CodePointInfraCategory.C0ControlOrSpace
        | CodePointInfraCategory.AsciiTabOrNewline
        | CodePointInfraCategory.AsciiWhitespace)]
    [InlineData(0x1F, CodePointInfraCategory.ScalarValue | CodePointInfraCategory.Ascii
        | CodePointInfraCategory.Control
        | CodePointInfraCategory.C0Control
        | CodePointInfraCategory.C0ControlOrSpace)]
    [InlineData(' ', CodePointInfraCategory.ScalarValue | CodePointInfraCategory.Ascii
        | CodePointInfraCategory.C0ControlOrSpace
        | CodePointInfraCategory.AsciiWhitespace)]
    [InlineData('0', CodePointInfraCategory.ScalarValue | CodePointInfraCategory.Ascii
        | CodePointInfraCategory.AsciiDigit
        | CodePointInfraCategory.AsciiUpperHexDigit
        | CodePointInfraCategory.AsciiLowerHexDigit
        | CodePointInfraCategory.AsciiAlphanumeric)]
    [InlineData('9', CodePointInfraCategory.ScalarValue | CodePointInfraCategory.Ascii
        | CodePointInfraCategory.AsciiDigit
        | CodePointInfraCategory.AsciiUpperHexDigit
        | CodePointInfraCategory.AsciiLowerHexDigit
        | CodePointInfraCategory.AsciiAlphanumeric)]
    [InlineData('A', CodePointInfraCategory.ScalarValue | CodePointInfraCategory.Ascii
        | CodePointInfraCategory.AsciiUpperHexDigit
        | CodePointInfraCategory.AsciiHexDigit
        | CodePointInfraCategory.AsciiUpperAlpha
        | CodePointInfraCategory.AsciiAlpha
        | CodePointInfraCategory.AsciiAlphanumeric)]
    [InlineData('F', CodePointInfraCategory.ScalarValue | CodePointInfraCategory.Ascii
        | CodePointInfraCategory.AsciiUpperHexDigit
        | CodePointInfraCategory.AsciiHexDigit
        | CodePointInfraCategory.AsciiUpperAlpha
        | CodePointInfraCategory.AsciiAlpha
        | CodePointInfraCategory.AsciiAlphanumeric)]
    [InlineData('Z', CodePointInfraCategory.ScalarValue | CodePointInfraCategory.Ascii
        | CodePointInfraCategory.AsciiUpperAlpha
        | CodePointInfraCategory.AsciiAlpha
        | CodePointInfraCategory.AsciiAlphanumeric)]
    [InlineData('a', CodePointInfraCategory.ScalarValue | CodePointInfraCategory.Ascii
        | CodePointInfraCategory.AsciiLowerHexDigit
        | CodePointInfraCategory.AsciiHexDigit
        | CodePointInfraCategory.AsciiLowerAlpha
        | CodePointInfraCategory.AsciiAlpha
        | CodePointInfraCategory.AsciiAlphanumeric)]
    [InlineData('f', CodePointInfraCategory.ScalarValue | CodePointInfraCategory.Ascii
        | CodePointInfraCategory.AsciiLowerHexDigit
        | CodePointInfraCategory.AsciiHexDigit
        | CodePointInfraCategory.AsciiLowerAlpha
        | CodePointInfraCategory.AsciiAlpha
        | CodePointInfraCategory.AsciiAlphanumeric)]
    [InlineData('z', CodePointInfraCategory.ScalarValue | CodePointInfraCategory.Ascii
        | CodePointInfraCategory.AsciiLowerAlpha
        | CodePointInfraCategory.AsciiAlpha
        | CodePointInfraCategory.AsciiAlphanumeric)]
    [InlineData(0x7F, CodePointInfraCategory.ScalarValue | CodePointInfraCategory.Ascii
        | CodePointInfraCategory.Control)]
    [InlineData(0x9F, CodePointInfraCategory.ScalarValue | CodePointInfraCategory.Control)]
    public static void CodePointsHaveCorrectInfraCategories(int codePoint, CodePointInfraCategory categories)
    {
        Assert.Equal(categories, new CodePoint(codePoint).InfraCategories);
    }

    public static readonly object[][] Utf8ValidTestCases = new object[][] {
        new object[] { "$", new CodePoint[] { 0x0024 } },
        new object[] { "£", new CodePoint[] { 0x00A3 } },
        new object[] { "ह", new CodePoint[] { 0x0939 } },
        new object[] { "€", new CodePoint[] { 0x20AC } },
        new object[] { "한", new CodePoint[] { 0xd55c } },
        new object[] { "𐍈", new CodePoint[] { 0x10348 } },
    };

    [Theory]
    [MemberData(nameof(Utf8ValidTestCases))]
    public static void DecodeUtf8FromEnumerableBytes(string value, CodePoint[] expected)
    {
        Assert.Equal(expected, CodePoint.DecodeUtf8(UTF8.GetBytes(value)).ToArray());
    }

    [Theory]
    [MemberData(nameof(Utf8ValidTestCases))]
    public static void EncodeUtf8FromEnumerableCodePoints(string expected, CodePoint[] value)
    {
        Assert.Equal(expected, UTF8.GetString(CodePoint.EncodeUtf8(value).ToArray()));
    }

    public static readonly object[][] Utf16TestCases = new object[][] {
        new object[] { "$", new CodePoint[] { 0x0024 } },
        new object[] { "€", new CodePoint[] { 0x20AC } },
        new object[] { "𐐷", new CodePoint[] { 0x10437 } },
        new object[] { "𤭢", new CodePoint[] { 0x24B62 } },
    };

    [Theory]
    [MemberData(nameof(Utf16TestCases))]
    public static void DecodeUtf16FromString(string value, CodePoint[] expected)
    {
        Assert.Equal(expected, CodePoint.DecodeUtf16(value).ToArray());
    }

    [Theory]
    [MemberData(nameof(Utf16TestCases))]
    public static void DecodeUtf16FromEnumerableCharacters(string value, CodePoint[] expected)
    {
        Assert.Equal(expected, CodePoint.DecodeUtf16((IEnumerable<char>)value).ToArray());
    }

    [Theory]
    [MemberData(nameof(Utf16TestCases))]
    public static void EncodeUtf16FromEnumerableCodePoints(string expected, CodePoint[] value)
    {
        Assert.Equal(expected, CodePoint.EncodeUtf16(value).ToArray());
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
    public static void ConvertToString(string expected, int codePoint)
    {
        Assert.Equal(expected, ((CodePoint)codePoint).ToString());
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
    public static void DecodeUtf16FromStackEnumerableCharacters(string value, CodePoint[] expected)
    {
        var results = new List<CodePoint>();
        foreach (var codePoint in CodePoint.GetEnumerable(value))
            results.Add(codePoint);

        Assert.Equal(expected, results);
    }
}