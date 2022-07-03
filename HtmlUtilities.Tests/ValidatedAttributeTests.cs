namespace HtmlUtilities;

public static class ValidatedAttributeTests
{
    [Theory]
    [InlineData("lang", "en", "lang=en")]
    [InlineData("lang", "en-us", "lang=en-us")]
    [InlineData("test", "top left", "test=\"top left\"")]
    [InlineData("test", "d&d", "test=d&amp;d")]
    [InlineData("test", "Dungeons & Dragons", "test=\"Dungeons &amp; Dragons\"")]
    public static void AttributeIsFormattedCorrectly(string name, string value, string expected)
    {
        Assert.Equal(expected, new ValidatedAttribute(name, value).ToString());
    }
}