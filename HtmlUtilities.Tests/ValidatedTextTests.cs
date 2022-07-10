namespace HtmlUtilities;

public static class ValidatedTextTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("Test", "Test")]
    [InlineData("D&D", "D&amp;D")]
    [InlineData("A<li>B", "A&lt;li>B")]
    public static void ValidatedTextIsFormattedCorrectly(string source, string expected)
    {
        Assert.Equal(expected, new ValidatedText(source).ToString());
    }
}
