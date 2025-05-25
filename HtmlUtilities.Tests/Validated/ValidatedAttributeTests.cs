namespace HtmlUtilities.Validated;

public static class ValidatedAttributeTests
{
    [Fact]
    public static void ValidatedAttributeUnconstructedIsBlank()
    {
        Assert.Empty(((ValidatedAttribute)default).ToString());
    }

    [Theory]
    [InlineData("lang", "en", " lang=en")]
    [InlineData("lang", "en-us", " lang=en-us")]
    [InlineData("test", "", " test")]
    [InlineData("test", null, " test")]
    [InlineData("test", "top left", " test=\"top left\"")]
    [InlineData("test", "d&d", " test=d&amp;d")]
    [InlineData("test", "Dungeons & Dragons", " test=\"Dungeons &amp; Dragons\"")]
    public static void AttributeIsFormattedCorrectly(string name, string? value, string expected)
    {
        Assert.Equal(expected, new ValidatedAttribute(name, value).ToString());

        Span<byte> utf8Name = stackalloc byte[RuneSmith.Utf8BytesNeeded(name)];
        Span<byte> utf8Value = stackalloc byte[RuneSmith.Utf8BytesNeeded(value)];

        RuneSmith.SwitchUtf(name, utf8Name);
        RuneSmith.SwitchUtf(value, utf8Value);

        Assert.Equal(expected, new ValidatedAttribute(utf8Name, utf8Value).ToString());
    }
}