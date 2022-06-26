using static System.Text.Encoding;

namespace HtmlUtilities.Tests;

public static class CodePointTests
{
    public static readonly object[][] Utf8ValidTestCases = new object[][] {
        new object[] { "$", new int[] { 0x0024 } },
        new object[] { "£", new int[] { 0x00A3 } },
        new object[] { "ह", new int[] { 0x0939 } },
        new object[] { "€", new int[] { 0x20AC } },
        new object[] { "한", new int[] { 0xd55c } },
        new object[] { "𐍈", new int[] { 0x10348 } },
    };

    [Theory]
    [MemberData(nameof(Utf8ValidTestCases))]
    public static void DecodeUtf8FromEnumerableBytes(string value, int[] expected)
    {
        Assert.Equal(expected, CodePoint.DecodeUtf8(UTF8.GetBytes(value)).ToArray());
    }

    [Theory]
    [MemberData(nameof(Utf8ValidTestCases))]
    public static void EncodeUtf8FromEnumerableCodePoints(string expected, int[] value)
    {
        Assert.Equal(expected, UTF8.GetString(CodePoint.EncodeUtf8(value).ToArray()));
    }
}