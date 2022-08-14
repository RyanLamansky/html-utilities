namespace HtmlUtilities.Validated;

public static class ValidatedAttributeNameTests
{
    [Fact]
    public static void ValidatedAttributeNameUnconstructedThrows()
    {
        var array = new ValidatedAttributeName[1];

        Assert.Throws<InvalidOperationException>(array[0].ToString);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(">")]
    [InlineData("a ")]
    public static void InvalidAttributeNameIsBlocked(string name)
    {
        _ = Assert.Throws<ArgumentException>(() => new ValidatedAttributeName(name));
    }

    [Theory]
    [InlineData("class")]
    [InlineData("CLASS")]
    [InlineData("Class")]
    [InlineData("x")]
    [InlineData("x1")]
    public static void ValidAttributeNameIsAccepted(string name)
    {
        _ = new ValidatedAttributeName(name);
    }
}