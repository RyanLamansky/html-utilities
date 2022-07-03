namespace HtmlUtilities;

public static class ValidatedAttributeValueTests
{
    [Theory]
    [InlineData("en", "=en")]
    [InlineData("en-us", "=en-us")]
    [InlineData("top left", "=\"top left\"")]
    [InlineData("d&d", "=d&amp;d")]
    [InlineData("Dungeons & Dragons", "=\"Dungeons &amp; Dragons\"")]
    public static void AttributeValueIsFormattedCorrectly(string source, string expected)
    {
        Assert.Equal(expected, new ValidatedAttributeValue(source).ToString());
    }
}