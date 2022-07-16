namespace HtmlUtilities;

public static class ValidatedTextTests
{
    [Fact]
    public static void ValidatedTextUnconstructedIsBlank()
    {
        var array = new ValidatedText[1];

        Assert.Empty(array[0].ToString());
    }

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
