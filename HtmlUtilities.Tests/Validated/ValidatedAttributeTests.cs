namespace HtmlUtilities.Validated;

public static class ValidatedAttributeTests
{
    [Fact]
    public static void ValidatedAttributeUnconstructedIsBlank()
    {
        var array = new ValidatedAttribute[1];

        Assert.Empty(array[0].ToString());
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
    }
}