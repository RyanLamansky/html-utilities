namespace HtmlUtilities.Validated;

public static class ValidatedAttributeNameTests
{
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