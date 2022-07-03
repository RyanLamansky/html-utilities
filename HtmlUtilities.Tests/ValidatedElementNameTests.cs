namespace HtmlUtilities;

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
        _ = Assert.Throws<ArgumentException>(() => new ValidatedElementName(name));
    }

    [Theory]
    [InlineData("html")]
    [InlineData("HTML")]
    [InlineData("Html")]
    [InlineData("a")]
    [InlineData("h1")]
    public static void ValidElementNameIsAccepted(string name)
    {
        _ = new ValidatedElementName(name);
    }
}