namespace HtmlUtilities.Validated;

public static class ValidatedElementTests
{

    [Fact]
    public static void ValidatedElementDiscardsUnconstructedAttributes()
    {
        var array = new ValidatedAttribute[2];
        array[0] = new("lang", "en");
        Assert.Equal("<html lang=en>", new ValidatedElement(new("html"), array).ToString());
    }

    [Fact]
    public static void ValidatedElementIsCorrect()
    {
        Assert.Equal("<html>", new ValidatedElement("html").ToString());
    }

    [Fact]
    public static void ValidatedElementWithAttributesIsCorrect()
    {
        Assert.Equal("<html lang=en-us>", new ValidatedElement(new("html"), [new("lang", "en-us")]).ToString());
    }
}
