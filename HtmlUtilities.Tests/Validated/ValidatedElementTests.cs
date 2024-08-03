namespace HtmlUtilities.Validated;

public static class ValidatedElementTests
{

    [Fact]
    public static void ValidatedElementDiscardsUnconstructedAttributes()
    {
        var array = new ValidatedAttribute[2];
        array[0] = ("lang", "en");
        Assert.Equal("<html lang=en>", new ValidatedElement("html", array).ToString());
    }

    [Fact]
    public static void ValidatedElementIsCorrect()
    {
        Assert.Equal("<html>", new ValidatedElement("html").ToString());
    }

    [Fact]
    public static void ValidatedElementWithAttributesIsCorrect()
    {
        Assert.Equal("<html lang=en-us>", new ValidatedElement("html", [("lang", "en-us")]).ToString());
    }
}
