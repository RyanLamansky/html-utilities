namespace HtmlUtilities.Validated;

public static class ValidatedElementNameTests
{
    [Fact]
    public static void ValidatedElementNameUnconstructedThrows()
    {
        var array = new ValidatedElementName[1];

        Assert.Throws<InvalidOperationException>(array[0].ToString);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1")]
    [InlineData("!")]
    [InlineData("a!")]
    [InlineData("a ")]
    [InlineData("Η")] // Greek Capital Letter Eta
    [InlineData("🙂")]
    [InlineData("script")]
    [InlineData("Script")]
    [InlineData("ScripT")]
    [InlineData("SCRIPT")]
    public static void InvalidElementNameIsBlocked(string name)
    {
        var x = Assert.Throws<ArgumentException>(() => new ValidatedElementName(name));
        Assert.Equal("name", x.ParamName);
    }

    [Theory]
    [InlineData("html")]
    [InlineData("HTML")]
    [InlineData("Html")]
    [InlineData("a")]
    [InlineData("h1")]
    public static void ValidElementNameStringIsAccepted(string name)
    {
        _ = new ValidatedElementName(name);
    }

    [Theory]
    [InlineData("html")]
    [InlineData("HTML")]
    [InlineData("Html")]
    [InlineData("a")]
    [InlineData("h1")]
    public static void ValidElementNameReadOnlySpanCharIsAccepted(string name)
    {
        _ = new ValidatedElementName(name.AsSpan());
    }
}