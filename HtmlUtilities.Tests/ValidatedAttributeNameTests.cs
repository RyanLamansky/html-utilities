namespace HtmlUtilities.Tests;

public static class ValidatedAttributeNameTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(">")]
    [InlineData("a ")]
    public static void InvalidAttributeNameIsBlocked(string name)
    {
        _ = Assert.Throws<Exception>(() => new ValidatedAttributeName(System.Text.Encoding.UTF8.GetBytes(name)));
    }

    [Theory]
    [InlineData("class")]
    [InlineData("CLASS")]
    [InlineData("Class")]
    [InlineData("x")]
    [InlineData("x1")]
    public static void ValidAttributeNameIsAccepted(string name)
    {
        _ = new ValidatedAttributeName(System.Text.Encoding.UTF8.GetBytes(name));
    }
}