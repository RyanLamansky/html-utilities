namespace HtmlUtilities.Validated;

public static class ValidatedElementTests
{
    [Fact]
    public static void ValidatedElementUnconstructedThrows()
    {
        var array = new ValidatedElement[1];

        Assert.Throws<InvalidOperationException>(array[0].ToString);
    }

    [Fact]
    public static void ValidatedElementThrowsForUnconstructedElementName()
    {
        var array = new ValidatedElementName[1];
        Assert.Equal("name", Assert.Throws<ArgumentException>(() => new ValidatedElement(array[0])).ParamName);
    }

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
