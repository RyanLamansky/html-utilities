using static HtmlUtilities.CodePointInfraCategory;
using static System.Text.Encoding;
using Rune = System.Text.Rune;

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
    [InlineData(0xFFFE, ScalarValue | NonCharacter)]
    public static void CodePointsHaveCorrectInfraCategories(int codePoint, CodePointInfraCategory categories)
    {
        Assert.Equal(categories, new Rune(codePoint).InfraCategories());
    }

    public static readonly object?[][] Utf8ValidTestCases = [
        [null, Array.Empty<Rune>()],
        ["$", new Rune[] { new(0x0024) }],
        ["£", new Rune[] { new(0x00A3) }],
        ["ह", new Rune[] { new(0x0939) }],
        ["€", new Rune[] { new(0x20AC) }],
        ["한", new Rune[] { new(0xd55c) }],
        ["𐍈", new Rune[] { new(0x10348) }],
    ];

    [Theory]
    [MemberData(nameof(Utf8ValidTestCases))]
    public static void DecodeUtf8FromEnumerableBytes(string? value, Rune[] expected)
    {
        Assert.Equal(expected, RuneSmith.DecodeUtf8(value is null ? null : UTF8.GetBytes(value)).ToArray());
    }

    [Theory]
    [MemberData(nameof(Utf8ValidTestCases))]
    public static void EncodeUtf8FromEnumerableCodePoints(string? expected, Rune[] value)
    {
        Assert.Equal(expected is null ? "" : expected, UTF8.GetString(RuneSmith.EncodeUtf8(value).ToArray()));
    }

    public static readonly object?[][] Utf16TestCases = [
        [null, Array.Empty<Rune>()],
        ["$", new Rune[] { new(0x0024) }],
        ["€", new Rune[] { new(0x20AC) }],
        ["𐐷", new Rune[] { new(0x10437) }],
        ["𤭢", new Rune[] { new(0x24B62) }],
    ];

    [Theory]
    [MemberData(nameof(Utf16TestCases))]
    public static void DecodeUtf16FromString(string? value, Rune[] expected)
    {
        Assert.Equal(expected, RuneSmith.DecodeUtf16(value).ToArray());
    }

    [Theory]
    [MemberData(nameof(Utf16TestCases))]
    public static void DecodeUtf16FromEnumerableCharacters(string? value, Rune[] expected)
    {
        Assert.Equal(expected, RuneSmith.DecodeUtf16((IEnumerable<char>?)value).ToArray());
    }

    [Theory]
    [MemberData(nameof(Utf16TestCases))]
    public static void EncodeUtf16FromEnumerableCodePoints(string? expected, Rune[] value)
    {
        Assert.Equal(expected is null ? "" : expected, new string(RuneSmith.EncodeUtf16(value).ToArray()));
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
        Assert.Equal(UTF8.GetByteCount(value), RuneSmith.DecodeUtf16(value).Sum(cp => cp.Utf8SequenceLength));
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
        Assert.Equal(Unicode.GetByteCount(value), RuneSmith.DecodeUtf16(value).Sum(cp => cp.Utf16SequenceLength * 2));
    }

    [Theory]
    [InlineData("$", 0x0024)]
    [InlineData("€", 0x20AC)]
    [InlineData("𐐷", 0x10437)]
    [InlineData("𤭢", 0x24B62)]
    public static void ConvertToString(string expected, int codePoint)
    {
        Assert.Equal(expected, ((Rune)codePoint).ToString());
    }

    [Theory]
    [MemberData(nameof(Utf16TestCases))]
    public static void DecodeUtf16FromStackString(string? value, Rune[] expected)
    {
        var results = new List<Rune>();
        foreach (var codePoint in RuneSmith.GetEnumerable(value))
            results.Add(codePoint);

        Assert.Equal(expected, results);
    }

    [Theory]
    [MemberData(nameof(Utf16TestCases))]
    public static void DecodeUtf16FromStackReadOnlySpan(string? value, Rune[] expected)
    {
        var results = new List<Rune>();
        foreach (var codePoint in RuneSmith.GetEnumerable(value.AsSpan()))
            results.Add(codePoint);

        Assert.Equal(expected, results);
    }

    [Theory]
    [InlineData("Test", 4)]
    [InlineData("日本語", 9)]
    public static void Utf8BytesNeeded(string value, int utf8ByteLength)
    {
        Assert.Equal(utf8ByteLength, RuneSmith.Utf8BytesNeeded(value));
    }

    [Theory]
    [InlineData("Test", new byte[] { 84, 101, 115, 116 })]
    [InlineData("日本語", new byte[] { 230, 151, 165, 230, 156, 172, 232, 170, 158 })]
    public static void SwitchUtf16To8(string value, byte[] expected)
    {
        var destination = new byte[RuneSmith.Utf8BytesNeeded(value)];

        RuneSmith.SwitchUtf(value, destination);

        Assert.Equal(expected, destination);
    }
}