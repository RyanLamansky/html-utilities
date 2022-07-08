namespace HtmlUtilities;

public static class ValidatedElementTests
{
    [Fact]
    public static void ValidatedElementIsCorrect()
    {
        Assert.Equal("<html>", new ValidatedElement("html").ToString());
    }

    [Fact]
    public static void ValidatedElementWithAttributesIsCorrect()
    {
        Assert.Equal("<html lang=en-us>", new ValidatedElement("html", new [] { new ValidatedAttribute("lang", "en-us")}).ToString());
    }
}
