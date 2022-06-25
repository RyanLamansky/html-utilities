namespace HtmlUtilities.Tests;

public static class ValidatedElementNameTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1")]
    [InlineData("!")]
    [InlineData("a!")]
    [InlineData("a ")]
    [InlineData("Η")] // Greek Capital Letter Eta
    [InlineData("🙂")]
    public static void InvalidElementNameIsBlocked(string name)
    {
        _ = Assert.Throws<Exception>(() => new ValidatedElementName(System.Text.Encoding.UTF8.GetBytes(name)));
    }

    [Theory]
    [InlineData("html")]
    [InlineData("HTML")]
    [InlineData("Html")]
    [InlineData("a")]
    [InlineData("h1")]
    public static void ValidElementNameIsAccepted(string name)
    {
        _ = new ValidatedElementName(System.Text.Encoding.UTF8.GetBytes(name));
    }
}