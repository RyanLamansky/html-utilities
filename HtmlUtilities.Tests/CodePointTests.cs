using static System.Text.Encoding;

namespace HtmlUtilities;

public static class CodePointTests
{
    [Theory]
    [InlineData(0x10FFFF + 1)]
    [InlineData(-1)]
    public static void OutOfRangeValuesAreBlocked(int value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CodePoint(value));
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
}